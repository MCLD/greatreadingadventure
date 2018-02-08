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
        : AuditingRepository<Model.Mail, Mail>, IMailRepository
    {
        public MailRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<MailRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<Mail>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                       && _.IsBroadcast == false
                       && _.SiteId == siteId)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Mail>()
                .ToListAsync();
        }

        public async Task<int> GetAllCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == siteId && _.IsBroadcast == false)
                .CountAsync();
        }

        public async Task<int> GetAdminUnrepliedCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.SiteId == siteId
                    && _.ToUserId == null
                    && _.IsRepliedTo == false)
                .CountAsync();
        }

        public async Task<IEnumerable<Mail>> PageAdminUnrepliedAsync(int siteId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.SiteId == siteId
                    && _.ToUserId == null
                    && _.IsRepliedTo == false)
                .Skip(skip)
                .Take(take)
                .ProjectTo<Mail>()
                .ToListAsync();
        }

        public async Task MarkAdminReplied(int mailId)
        {
            var mail = await GetByIdAsync(mailId);
            if (mail == null)
            {
                throw new Exception($"Mail id {mailId} not found.");
            }
            mail.IsRepliedTo = true;
            await UpdateSaveNoAuditAsync(mail);
        }

        public async Task<int> GetUserCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && (_.ToUserId == userId || _.FromUserId == userId))
                .CountAsync();
        }

        public async Task<int> GetUserInboxCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.ToUserId == userId)
                .CountAsync();
        }
        public async Task<IEnumerable<Mail>> PageUserAsync(int userId, int skip, int take)
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

        public async Task<IEnumerable<Mail>> PageUserInboxAsync(int userId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false
                    && _.ToUserId == userId)
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
                _logger.LogError($"Could not find mail id {mailId}");
                throw new Exception($"Could not find mail id {mailId}");
            }
            mail.IsNew = false;
            await SaveAsync();
        }

        public override async Task<Mail> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .ProjectTo<Mail>()
                .SingleOrDefaultAsync();
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

        public async Task<bool> UserHasUnreadAsync(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.ToUserId == userId && _.IsNew == true && _.IsDeleted == false)
                .AnyAsync();
        }

        public async Task<List<Mail>> GetThreadAsync(int threadId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Id == threadId || _.ThreadId == threadId)
                .OrderBy(_ => _.CreatedAt)
                .ProjectTo<Mail>()
                .ToListAsync();
        }
    }
}
