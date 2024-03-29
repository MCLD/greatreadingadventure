﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Abstract;
using GRA.Data.Abstract;
using GRA.Data.Model;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data
{
    public class AuditingRepository<DbEntity, DomainEntity>
        where DbEntity : BaseDbEntity
        where DomainEntity : Domain.Model.Abstract.BaseDomainEntity
    {
        protected readonly IConfiguration _config;
        protected readonly Context _context;
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly IEntitySerializer _entitySerializer;
        protected readonly ILogger _logger;
        protected readonly AutoMapper.IMapper _mapper;
        private readonly DbSet<AuditLog> _auditSet;
        private DbSet<DbEntity> _dbSet;

        internal AuditingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<IRepository<DomainEntity>> logger)
        {
            if (repositoryFacade == null)
            {
                throw new ArgumentNullException(nameof(repositoryFacade));
            }
            _context = repositoryFacade.context;
            _mapper = repositoryFacade.mapper;
            _config = repositoryFacade.config;
            _dateTimeProvider = repositoryFacade.dateTimeProvider;
            _entitySerializer = repositoryFacade.entitySerializer;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_config["SuppressAuditLog"]))
            {
                _auditSet = _context.Set<AuditLog>();
            }
        }

        protected DbSet<AuditLog> AuditSet
        {
            get
            {
                return _auditSet;
            }
        }

        protected DbSet<DbEntity> DbSet
        {
            get
            {
                return _dbSet ??= _context.Set<DbEntity>();
            }
        }

        public virtual async Task AddAsync(int userId, DomainEntity domainEntity)
        {
            await AddAsync(userId, _mapper.Map<DomainEntity, DbEntity>(domainEntity));
        }

        public virtual async Task<DomainEntity> AddSaveAsync(int userId, DomainEntity domainEntity)
        {
            var dbEntity = _mapper.Map<DomainEntity, DbEntity>(domainEntity);
            return await AddSaveAsync(userId, dbEntity);
        }

        public virtual async Task<DomainEntity> AddSaveNoAuditAsync(DomainEntity domainEntity)
        {
            var dbEntity = _mapper.Map<DbEntity>(domainEntity);
            await DbSet.AddAsync(dbEntity);
            await SaveAsync();
            return await GetByIdAsync(dbEntity.Id);
        }

        public virtual async Task<DomainEntity> GetByIdAsync(int id)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
            if (entity == null)
            {
                throw new GraException($"{nameof(DomainEntity)} id {id} could not be found.");
            }
            return _mapper.Map<DbEntity, DomainEntity>(entity);
        }

        public async Task<ICollection<ChangedItem<DomainEntity>>> GetChangesAsync(int entityId)
        {
            var entityType = typeof(DbEntity).FullName;
            var items = await _context.AuditLogs
                .AsNoTracking()
                .Where(_ => _.EntityId == entityId && _.EntityType == entityType)
                .ToListAsync();

            var changedItems = new List<ChangedItem<DomainEntity>>();

            foreach (var item in items)
            {
                changedItems.Add(new ChangedItem<DomainEntity>
                {
                    ChangedAt = item.UpdatedAt,
                    ChangedByUserId = item.UpdatedBy,
                    OldItem = JsonSerializer.Deserialize<DomainEntity>(item.PreviousValue),
                    NewItem = JsonSerializer.Deserialize<DomainEntity>(item.CurrentValue)
                });
            }

            return changedItems;
        }

        public virtual async Task<IEnumerable<DomainEntity>> PageAllAsync(int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Id)
                .Skip(skip)
                .Take(take)
                .ProjectTo<DomainEntity>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public virtual async Task RemoveAsync(int userId, int id)
        {
            var entity = await DbSet.FindAsync(id)
                ?? throw new GraException($"{nameof(DomainEntity)} id {id} could not be found.");
            DbSet.Remove(entity);
            if (AuditSet != null)
            {
                var audit = new AuditLog
                {
                    EntityType = entity.GetType().ToString(),
                    EntityId = entity.Id,
                    UpdatedBy = userId,
                    UpdatedAt = _dateTimeProvider.Now,
                    CurrentValue = null,
                    PreviousValue = _entitySerializer.Serialize(entity)
                };
                await AuditSet.AddAsync(audit);
            }
        }

        public virtual async Task RemoveSaveAsync(int userId, int id)
        {
            await RemoveAsync(userId, id);
            await SaveAsync();
        }

        public virtual async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbupex)
            {
                _logger.LogError("An error occurred in SaveAsync: {ErrorMessage}", dbupex.Message);
                if (dbupex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {ErrorMessage}",
                        dbupex.InnerException.Message);
                }
                throw new GraDbUpdateException(dbupex.Message, dbupex);
            }
        }

        public virtual async Task UpdateAsync(int userId, DomainEntity domainEntity)
        {
            var dbEntity = await DbSet.FindAsync(domainEntity.Id);
            string original = null;
            if (AuditSet != null)
            {
                original = _entitySerializer.Serialize(dbEntity);
            }
            _mapper.Map<DomainEntity, DbEntity>(domainEntity, dbEntity);
            await UpdateAsync(userId, dbEntity, original);
        }

        public virtual async Task<DomainEntity> UpdateSaveAsync(int userId,
            DomainEntity domainEntity)
        {
            await UpdateAsync(userId, domainEntity);
            await SaveAsync();
            return await GetByIdAsync(domainEntity.Id);
        }

        public virtual async Task<DomainEntity> UpdateSaveNoAuditAsync(DomainEntity domainEntity)
        {
            var dbEntity = await DbSet.FindAsync(domainEntity.Id);
            var created = new Tuple<int, DateTime>(dbEntity.CreatedBy, dbEntity.CreatedAt);
            _mapper.Map<DomainEntity, DbEntity>(domainEntity, dbEntity);
            dbEntity.CreatedBy = created.Item1;
            dbEntity.CreatedAt = created.Item2;
            DbSet.Update(dbEntity);
            await SaveAsync();
            return await GetByIdAsync(domainEntity.Id);
        }

        protected virtual async Task AddAsync(int userId, DbEntity dbEntity)
        {
            dbEntity.CreatedBy = userId;
            dbEntity.CreatedAt = _dateTimeProvider.Now;
            EntityEntry<DbEntity> dbEntityEntry = _context.Entry(dbEntity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await DbSet.AddAsync(dbEntity);
            }
        }

        protected virtual async Task<DomainEntity> AddSaveAsync(int userId, DbEntity dbEntity)
        {
            await AddAsync(userId, dbEntity);
            await SaveAsync();
            return _mapper.Map<DbEntity, DomainEntity>(dbEntity);
        }

        protected IQueryable<DbEntity> ApplyPagination(IQueryable<DbEntity> queryable, BaseFilter filter)
        {
            if (filter.Skip != null && filter.Take != null)
            {
                return queryable.Skip((int)filter.Skip).Take((int)filter.Take);
            }
            else
            {
                return queryable;
            }
        }

        protected async Task AuditLog(int currentUserId,
            BaseDbEntity newObject,
            BaseDbEntity priorObject)
        {
            await AuditLog(currentUserId, newObject.Id, newObject, priorObject);
        }

        protected async Task AuditLog(int currentUserId,
            BaseDbEntity newObject,
            string priorObjectSerialized)
        {
            await AuditLog(currentUserId, newObject.Id, newObject, priorObjectSerialized, true);
        }

        protected virtual async Task UpdateAsync(int userId, DbEntity dbEntity, string original)
        {
            var created = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == dbEntity.Id)
                .Select(_ => new Tuple<int, DateTime>(_.CreatedBy, _.CreatedAt))
                .SingleOrDefaultAsync();
            dbEntity.CreatedBy = created.Item1;
            dbEntity.CreatedAt = created.Item2;
            DbSet.Update(dbEntity);
            await AuditLog(userId, dbEntity, original);
        }

        protected virtual async Task<DomainEntity> UpdateSaveAsync(int userId,
            DbEntity dbEntity,
            string original)
        {
            await UpdateAsync(userId, dbEntity, original);
            await SaveAsync();
            return await GetByIdAsync(dbEntity.Id);
        }

        private async Task AuditLog(int currentUserId,
                            int objectId,
            object newObject,
            object priorObject,
            bool priorObjectAlreadySerialized = false)
        {
            if (_auditSet == null)
            {
                // audit logging is not enabled
                return;
            }
            var audit = new AuditLog
            {
                EntityType = newObject.GetType().ToString(),
                EntityId = objectId,
                UpdatedBy = currentUserId,
                UpdatedAt = _dateTimeProvider.Now,
                CurrentValue = _entitySerializer.Serialize(newObject)
            };
            if (priorObject != null)
            {
                if (priorObjectAlreadySerialized)
                {
                    audit.PreviousValue = priorObject.ToString();
                }
                else
                {
                    audit.PreviousValue = _entitySerializer.Serialize(priorObject);
                }
            }
            await AuditSet.AddAsync(audit);
        }
    }
}
