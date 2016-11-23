using AutoMapper.QueryableExtensions;
using GRA.Data.Abstract;
using GRA.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

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

        protected void AuditLog(int currentUserId,
            BaseDbEntity newObject,
            BaseDbEntity priorObject)
        {
            AuditLog(currentUserId, newObject.Id, newObject, priorObject);
        }

        protected void AuditLog(int currentUserId,
            BaseDbEntity newObject,
            string priorObjectSerialized)
        {
            AuditLog(currentUserId, newObject.Id, newObject, priorObjectSerialized, true);
        }

        private void AuditLog(int currentUserId,
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
            AuditSet.Add(audit);
        }

        private string SerializeEntity(object entity)
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

        public virtual IQueryable<DomainEntity> PageAll(int skip, int take)
        {
            return DbSet.AsNoTracking().Skip(skip).Take(take).ProjectTo<DomainEntity>();
        }

        public virtual DomainEntity GetById(int id)
        {
            var entity = DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id)
                .SingleOrDefault();
            if (entity == null)
            {
                throw new Exception($"{nameof(DomainEntity)} id {id} could not be found.");
            }
            return mapper.Map<DbEntity, DomainEntity>(entity);
        }

        public virtual void Add(int userId, DomainEntity domainEntity)
        {
            Add(userId, mapper.Map<DomainEntity, DbEntity>(domainEntity));
        }

        public virtual DomainEntity AddSave(int userId, DomainEntity domainEntity)
        {
            var dbEntity = mapper.Map<DomainEntity, DbEntity>(domainEntity);
            return AddSave(userId, dbEntity);
        }

        protected virtual DomainEntity AddSave(int userId, DbEntity dbEntity)
        {
            Add(userId, dbEntity);
            Save();
            return mapper.Map<DbEntity, DomainEntity>(dbEntity);
        }

        protected virtual void Add(int userId, DbEntity dbEntity)
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

        public virtual void Update(int userId, DomainEntity domainEntity)
        {
            var dbEntity = DbSet.Find(domainEntity.Id);
            string original = null;
            if (AuditSet != null)
            {
                original = SerializeEntity(dbEntity);
            }
            mapper.Map<DomainEntity, DbEntity>(domainEntity, dbEntity);
            Update(userId, dbEntity, original);
        }

        protected virtual void Update(int userId, DbEntity dbEntity, string original)
        {
            DbSet.Update(dbEntity);
            AuditLog(userId, dbEntity, original);
        }

        public virtual DomainEntity UpdateSave(int userId, DomainEntity domainEntity)
        {
            Update(userId, domainEntity);
            Save();
            return GetById(domainEntity.Id);
        }

        protected virtual DomainEntity UpdateSave(int userId, DbEntity dbEntity, string original)
        {
            Update(userId, dbEntity, original);
            Save();
            return GetById(dbEntity.Id);
        }

        public virtual void RemoveSave(int userId, int id)
        {
            var entity = DbSet.Find(id);
            if (entity == null)
            {
                throw new Exception($"{nameof(DomainEntity)} id {id} could not be found.");
            }
            DbSet.Remove(entity);
            Save();
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }
    }
}