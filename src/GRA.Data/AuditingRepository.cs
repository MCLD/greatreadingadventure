using AutoMapper.QueryableExtensions;
using GRA.Data.Abstract;
using GRA.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data
{
    public class AuditingRepository<DbEntity, DomainEntity>
        where DbEntity : BaseDbEntity
        where DomainEntity : Domain.Model.Abstract.BaseDomainEntity
    {
        protected readonly Context context;
        protected readonly ILogger logger;
        protected readonly AutoMapper.IMapper mapper;
        protected readonly IConfigurationRoot config;

        private DbSet<DbEntity> dbSet;
        private DbSet<AuditLog> auditSet;

        internal AuditingRepository(ServiceFacade.Repository repositoryFacade, ILogger logger)
        {
            if (repositoryFacade == null)
            {
                throw new ArgumentNullException(nameof(repositoryFacade));
            }
            this.context = repositoryFacade.context;
            this.mapper = repositoryFacade.mapper;
            this.config = repositoryFacade.config;
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;

            if (string.IsNullOrWhiteSpace(config["SuppressAuditLog"]))
            {
                auditSet = context.Set<AuditLog>();
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

        private async Task AuditLog(int currentUserId,
            int objectId,
            object newObject,
            object priorObject,
            bool priorObjectAlreadySerialized = false)
        {
            if (auditSet == null)
            {
                // audit logging is not enabled
                return;
            }
            var audit = new AuditLog
            {
                EntityType = newObject.GetType().ToString(),
                EntityId = objectId,
                UpdatedBy = currentUserId,
                UpdatedAt = DateTime.Now,
                CurrentValue = SerializeEntity(newObject)
            };
            if (priorObject != null)
            {
                if (priorObjectAlreadySerialized)
                {
                    audit.PreviousValue = priorObject.ToString();
                }
                else
                {
                    audit.PreviousValue = SerializeEntity(priorObject);
                }
            }
            await AuditSet.AddAsync(audit);
        }

        protected string SerializeEntity(object entity)
        {
            return JsonConvert.SerializeObject(entity,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        protected DbSet<AuditLog> AuditSet {
            get {
                return auditSet;
            }
        }
        protected DbSet<DbEntity> DbSet {
            get {
                return dbSet ?? (dbSet = context.Set<DbEntity>());
            }
        }

        public async virtual Task<ICollection<DomainEntity>> PageAllAsync(int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ProjectTo<DomainEntity>()
                .ToListAsync();
        }

        public async virtual Task<DomainEntity> GetByIdAsync(int id)
        {
            var entity = await DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
            if (entity == null)
            {
                throw new Exception($"{nameof(DomainEntity)} id {id} could not be found.");
            }
            return mapper.Map<DbEntity, DomainEntity>(entity);
        }

        public virtual async Task AddAsync(int userId, DomainEntity domainEntity)
        {
            await AddAsync(userId, mapper.Map<DomainEntity, DbEntity>(domainEntity));
        }

        public virtual async Task<DomainEntity> AddSaveAsync(int userId, DomainEntity domainEntity)
        {
            var dbEntity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            return await AddSaveAsync(userId, dbEntity);
        }

        protected virtual async Task<DomainEntity> AddSaveAsync(int userId, DbEntity dbEntity)
        {
            await AddAsync(userId, dbEntity);
            await SaveAsync();
            return mapper.Map<DbEntity, DomainEntity>(dbEntity);
        }

        protected virtual async Task AddAsync(int userId, DbEntity dbEntity)
        {
            dbEntity.CreatedBy = userId;
            dbEntity.CreatedAt = DateTime.Now;
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(dbEntity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                await DbSet.AddAsync(dbEntity);
            }
        }


        protected virtual async Task UpdateAsync(int userId, DbEntity dbEntity, string original)
        {
            DbSet.Update(dbEntity);
            await AuditLog(userId, dbEntity, original);
        }

        public virtual async Task<DomainEntity> UpdateSaveAsync(int userId,
            DomainEntity domainEntity)
        {
            await UpdateAsync(userId, domainEntity);
            await SaveAsync();
            return await GetByIdAsync(domainEntity.Id);
        }

        protected virtual async Task<DomainEntity> UpdateSaveAsync(int userId,
            DbEntity dbEntity,
            string original)
        {
            await UpdateAsync(userId, dbEntity, original);
            await SaveAsync();
            return await GetByIdAsync(dbEntity.Id);
        }

        public virtual async Task UpdateAsync(int userId, DomainEntity domainEntity)
        {
            var dbEntity = await DbSet.FindAsync(domainEntity.Id);
            string original = null;
            if (AuditSet != null)
            {
                original = SerializeEntity(dbEntity);
            }
            mapper.Map<DomainEntity, DbEntity>(domainEntity, dbEntity);
            await UpdateAsync(userId, dbEntity, original);
        }

        public virtual async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"{nameof(DomainEntity)} id {id} could not be found.");
            }
            DbSet.Remove(entity);
            await SaveAsync();
        }

        public virtual async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}