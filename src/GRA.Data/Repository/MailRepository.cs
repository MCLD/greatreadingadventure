using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class MailRepository
        : AuditingRepository<Model.Mail, Domain.Model.Mail>, IMailRepository
    {
        public MailRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<MailRepository> logger) : base(repositoryFacade, logger)
        {
        }
        
        public async Task<int> GetAllCountAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false)
                .CountAsync();
        }

        public async Task<int> GetAdminUnreadCountAsync()
        {
            int to = default(int);
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.ToUserId == to
                    && _.IsNew == true)
                .CountAsync();
        }

        public async Task<ICollection<Mail>> PageAdminUnreadAsync(int skip, int take)
        {
            int to = default(int);
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.ToUserId == to
                    && _.IsNew == true)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Mail>()
                .ToListAsync();
        }

        public async Task<int> GetUserCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && (_.ToUserId == userId || _.FromUserId == userId))
                .CountAsync();
        }
        public async Task<ICollection<Mail>> PageUserAsync(int userId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && (_.ToUserId == userId || _.FromUserId == userId))
                .Skip(skip)
                .Take(take)
                .ProjectTo<Mail>()
                .ToListAsync();
        }

        public async Task<int> GetUserUnreadCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.ToUserId == userId
                    && _.IsNew == true)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int mailId)
        {
            var mail = DbSet
                .Where(_ => _.Id == mailId)
                .SingleOrDefault();
            if (mail == null)
            {
                logger.LogError($"Could not find mail id {mailId}");
                throw new Exception($"Could not find mail id {mailId}");
            }
            mail.IsNew = false;
            await SaveAsync();
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
