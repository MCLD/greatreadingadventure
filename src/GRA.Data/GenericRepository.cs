using AutoMapper.QueryableExtensions;
using GRA.Data.Abstract;
using GRA.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace GRA.Data
{
    internal class GenericRepository<DbEntity, DomainEntity>
        where DbEntity : BaseDbEntity
        where DomainEntity : class
    {
        private readonly Context context;
        private readonly ILogger logger;
        private readonly AutoMapper.IMapper mapper;

        private DbSet<DbEntity> dbSet;
        private DbSet<AuditLog> auditSet;

        internal GenericRepository(Context context,
            ILogger logger,
           AutoMapper.IMapper mapper,
            bool writeAuditLog)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.context = context;
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            this.mapper = mapper;
            if(writeAuditLog)
            {
                auditSet = context.Set<AuditLog>();
            }
        }

        private void AuditLog(int userId,
            BaseDbEntity newObject,
            BaseDbEntity priorObject = null)
        {
            if(auditSet == null)
            {
                // audit logging is not enabled
                return;
            }
            var audit = new AuditLog
            {
                EntityType = newObject.GetType().ToString(),
                EntityId = newObject.Id,
                UpdatedBy = userId,
                UpdatedAt = DateTime.Now,
                CurrentValue = JsonConvert.SerializeObject(newObject)
            };
            if (priorObject != null)
            {
                audit.PreviousValue = JsonConvert.SerializeObject(priorObject);
            }
            AuditSet.Add(audit);
            try
            {
                if (context.SaveChanges() != 1)
                {
                    logger.LogError($"Error writing audit log for {newObject.GetType()} id {newObject.Id}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(null, ex, $"Error writing audit log for {newObject.GetType()} id {newObject.Id}");
            }
        }

        protected DbSet<AuditLog> AuditSet {
            get {
                return auditSet;
            }
        }
        public DbSet<DbEntity> DbSet {
            get {
                return dbSet ?? (dbSet = context.Set<DbEntity>());
            }
        }

        public IQueryable<DomainEntity> PageAll(int skip, int take)
        {
            return DbSet.AsNoTracking().Skip(skip).Take(take).ProjectTo<DomainEntity>();
        }

        public virtual DomainEntity GetById(int id)
        {
            return mapper.Map<DbEntity, DomainEntity>(DbSet.Find(id));
        }

        public void Add(int userId, DomainEntity domainEntity)
        {
            Add(userId, Map(domainEntity));
        }

        public DomainEntity AddSave(int userId, DomainEntity domainEntity)
        {
            var dbEntity = Map(domainEntity);
            return AddSave(userId, dbEntity);
        }

        public DomainEntity AddSave(int userId, DbEntity dbEntity)
        {
            Add(userId, dbEntity);
            Save();
            return Map(dbEntity);
        }

        public void Add(int userId, DbEntity dbEntity)
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
                DbSet.Add(dbEntity);
            }
        }

        public void Update(int userId, DomainEntity domainEntity)
        {
            var dbEntity = Map(domainEntity);
            Update(userId, dbEntity);
        }

        public DomainEntity UpdateSave(int userId, DomainEntity domainEntity)
        {
            var dbEntity = Map(domainEntity);
            return UpdateSave(userId, dbEntity);
        }
        public DomainEntity UpdateSave(int userId, DbEntity dbEntity)
        {
            Update(userId, dbEntity);
            return Map(dbEntity);
        }

        public void Update(int userId, DbEntity dbEntity)
        {
            var original = DbSet.Find(dbEntity.Id);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(dbEntity);
            if (dbEntityEntry.State != (EntityState)EntityState.Detached)
            {
                DbSet.Attach(dbEntity);
            }
            dbEntityEntry.State = EntityState.Modified;
            AuditLog(userId, dbEntity, original);
        }

        public void Remove(int userId, DomainEntity domainEntity)
        {
            DbEntity entity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            EntityEntry<DbEntity> dbEntityEntry = context.Entry(entity);
            if (dbEntityEntry.State != (EntityState)EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
            AuditLog(userId, null, entity);
        }

        public void Remove(int userId, int id)
        {
            var entity = GetById(id);
            if (entity == null) return;

            Remove(userId, entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public DbEntity Map(DomainEntity domainEntity)
        {
            return mapper.Map<DomainEntity, DbEntity>(domainEntity);
        }

        public DomainEntity Map(DbEntity dbEntity)
        {
            return mapper.Map<DbEntity, DomainEntity>(dbEntity);
        }
    }
}