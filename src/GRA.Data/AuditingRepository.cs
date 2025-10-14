using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Data.Abstract;
using GRA.Data.Model;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data
{
    public class AuditingRepository<TDbEntity, TDomainEntity>
        where TDbEntity : BaseDbEntity
        where TDomainEntity : Domain.Model.Abstract.BaseDomainEntity
    {
        protected readonly IConfiguration _config;
        protected readonly Context _context;
        protected readonly IDateTimeProvider _dateTimeProvider;
        protected readonly IEntitySerializer _entitySerializer;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        private DbSet<TDbEntity> _dbSet;

        internal AuditingRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<IRepository<TDomainEntity>> logger)
        {
            ArgumentNullException.ThrowIfNull(repositoryFacade);
            ArgumentNullException.ThrowIfNull(logger);

            _context = repositoryFacade.context;
            _mapper = repositoryFacade.mapper;
            _config = repositoryFacade.config;
            _dateTimeProvider = repositoryFacade.dateTimeProvider;
            _entitySerializer = repositoryFacade.entitySerializer;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_config["SuppressAuditLog"]))
            {
                AuditSet = _context.Set<AuditLog>();
            }
        }

        protected DbSet<AuditLog> AuditSet { get; }

        protected DbSet<TDbEntity> DbSet
        {
            get
            {
                return _dbSet ??= _context.Set<TDbEntity>();
            }
        }

        public virtual async Task AddAsync(int userId, TDomainEntity domainEntity)
        {
            await AddAsync(userId, _mapper.Map<TDomainEntity, TDbEntity>(domainEntity));
        }

        public virtual async Task<TDomainEntity> AddSaveAsync(int userId, TDomainEntity domainEntity)
        {
            var dbEntity = _mapper.Map<TDomainEntity, TDbEntity>(domainEntity);
            return await AddSaveAsync(userId, dbEntity);
        }

        public virtual async Task<TDomainEntity> AddSaveNoAuditAsync(TDomainEntity domainEntity)
        {
            var dbEntity = _mapper.Map<TDbEntity>(domainEntity);
            await DbSet.AddAsync(dbEntity);
            await SaveAsync();
            return await GetByIdAsync(dbEntity.Id);
        }

        public virtual async Task<TDomainEntity> GetByIdAsync(int id)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
            return entity == null
                ? throw new GraException($"{nameof(TDomainEntity)} id {id} could not be found.")
                : _mapper.Map<TDbEntity, TDomainEntity>(entity);
        }

        public async Task<ICollection<ChangedItem<TDomainEntity>>> GetChangesAsync(int entityId)
        {
            var entityType = typeof(TDbEntity).FullName;
            var items = await _context.AuditLogs
                .AsNoTracking()
                .Where(_ => _.EntityId == entityId && _.EntityType == entityType)
                .ToListAsync();

            var changedItems = new List<ChangedItem<TDomainEntity>>();

            foreach (var item in items)
            {
                changedItems.Add(new ChangedItem<TDomainEntity>
                {
                    ChangedAt = item.UpdatedAt,
                    ChangedByUserId = item.UpdatedBy,
                    OldItem = JsonSerializer.Deserialize<TDomainEntity>(item.PreviousValue),
                    NewItem = JsonSerializer.Deserialize<TDomainEntity>(item.CurrentValue)
                });
            }

            return changedItems;
        }

        public virtual async Task<IEnumerable<TDomainEntity>> PageAllAsync(int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .OrderBy(_ => _.Id)
                .Skip(skip)
                .Take(take)
                .ProjectToType<TDomainEntity>()
                .ToListAsync();
        }

        public virtual async Task RemoveAsync(int userId, int id)
        {
            var entity = await DbSet.FindAsync(id)
                ?? throw new GraException($"{nameof(TDomainEntity)} id {id} could not be found.");
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

        public virtual async Task UpdateAsync(int userId, TDomainEntity domainEntity)
        {
            ArgumentNullException.ThrowIfNull(domainEntity);
            var dbEntity = await DbSet.FindAsync(domainEntity.Id);
            string original = null;
            if (AuditSet != null)
            {
                original = _entitySerializer.Serialize(dbEntity);
            }
            _mapper.Map<TDomainEntity, TDbEntity>(domainEntity, dbEntity);
            await UpdateAsync(userId, dbEntity, original);
        }

        public virtual async Task<TDomainEntity> UpdateSaveAsync(int userId,
            TDomainEntity domainEntity)
        {
            ArgumentNullException.ThrowIfNull(domainEntity);
            await UpdateAsync(userId, domainEntity);
            await SaveAsync();
            return await GetByIdAsync(domainEntity.Id);
        }

        public virtual async Task<TDomainEntity> UpdateSaveNoAuditAsync(TDomainEntity domainEntity)
        {
            ArgumentNullException.ThrowIfNull(domainEntity);
            var dbEntity = await DbSet.FindAsync(domainEntity.Id);
            var created = new Tuple<int, DateTime>(dbEntity.CreatedBy, dbEntity.CreatedAt);
            _mapper.Map<TDomainEntity, TDbEntity>(domainEntity, dbEntity);
            dbEntity.CreatedBy = created.Item1;
            dbEntity.CreatedAt = created.Item2;
            DbSet.Update(dbEntity);
            await SaveAsync();
            return await GetByIdAsync(domainEntity.Id);
        }

        protected virtual async Task AddAsync(int userId, TDbEntity dbEntity)
        {
            ArgumentNullException.ThrowIfNull(dbEntity);
            dbEntity.CreatedBy = userId;
            dbEntity.CreatedAt = _dateTimeProvider.Now;
            EntityEntry<TDbEntity> dbEntityEntry = _context.Entry(dbEntity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await DbSet.AddAsync(dbEntity);
            }
        }

        protected virtual async Task<TDomainEntity> AddSaveAsync(int userId, TDbEntity dbEntity)
        {
            await AddAsync(userId, dbEntity);
            await SaveAsync();
            return _mapper.Map<TDbEntity, TDomainEntity>(dbEntity);
        }

        protected IQueryable<TDbEntity> ApplyPagination(IQueryable<TDbEntity> queryable,
            BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return filter.Skip != null && filter.Take != null
                ? queryable.Skip((int)filter.Skip).Take((int)filter.Take)
                : queryable;
        }

        protected async Task AuditLog(int currentUserId,
            BaseDbEntity newObject,
            BaseDbEntity priorObject)
        {
            ArgumentNullException.ThrowIfNull(newObject);
            await AuditLog(currentUserId, newObject.Id, newObject, priorObject);
        }

        protected async Task AuditLog(int currentUserId,
            BaseDbEntity newObject,
            string priorObjectSerialized)
        {
            ArgumentNullException.ThrowIfNull(newObject);
            await AuditLog(currentUserId, newObject.Id, newObject, priorObjectSerialized, true);
        }

        protected virtual async Task UpdateAsync(int userId, TDbEntity dbEntity, string original)
        {
            ArgumentNullException.ThrowIfNull(dbEntity);
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

        protected virtual async Task<TDomainEntity> UpdateSaveAsync(int userId,
            TDbEntity dbEntity,
            string original)
        {
            ArgumentNullException.ThrowIfNull(dbEntity);
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
            if (AuditSet == null)
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
