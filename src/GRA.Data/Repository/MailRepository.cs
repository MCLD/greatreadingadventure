using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class MailRepository(ServiceFacade.Repository repositoryFacade,
        ILogger<MailRepository> logger)
                : AuditingRepository<Model.Mail, Mail>(repositoryFacade, logger), IMailRepository
    {
        public async Task<IEnumerable<Mail>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && !_.IsBroadcast && _.SiteId == siteId)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectToType<Mail>()
                .ToListAsync();
        }

        public async Task<int> GetAllCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == siteId && !_.IsBroadcast)
                .CountAsync();
        }

        public async Task<int> GetAdminUnrepliedCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.SiteId == siteId
                    && _.ToUserId == null
                    && !_.IsRepliedTo)
                .CountAsync();
        }

        public async Task<IEnumerable<Mail>> PageAdminUnrepliedAsync(int siteId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.SiteId == siteId
                    && _.ToUserId == null
                    && !_.IsRepliedTo)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectToType<Mail>()
                .ToListAsync();
        }

        public async Task MarkAdminReplied(int mailId)
        {
            var mail = await GetByIdAsync(mailId)
                ?? throw new GraException($"Mail id {mailId} not found.");
            mail.IsRepliedTo = true;
            await UpdateSaveNoAuditAsync(mail);
        }

        public async Task<int> GetUserCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && (_.ToUserId == userId || _.FromUserId == userId))
                .CountAsync();
        }

        public async Task<int> GetUserInboxCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.ToUserId == userId)
                .CountAsync();
        }
        public async Task<IEnumerable<Mail>> PageUserAsync(int userId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && (_.ToUserId == userId || _.FromUserId == userId))
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectToType<Mail>()
                .ToListAsync();
        }

        public async Task<IEnumerable<Mail>> PageUserInboxAsync(int userId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.ToUserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectToType<Mail>()
                .ToListAsync();
        }

        public async Task<int> GetUserUnreadCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.ToUserId == userId && _.IsNew)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int mailId)
        {
            var mail = DbSet
                .Where(_ => _.Id == mailId)
                .SingleOrDefault();
            if (mail == null)
            {
                _logger.LogError("Could not find mail id {MailId}", mailId);
                throw new GraException($"Could not find mail id {mailId}");
            }
            mail.IsNew = false;
            await SaveAsync();
        }

        public override async Task<Mail> GetByIdAsync(int id)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.Id == id)
                .ProjectToType<Mail>()
                .SingleOrDefaultAsync();
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => !_.IsDeleted && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }

        public async Task<bool> UserHasUnreadAsync(int userId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.ToUserId == userId && _.IsNew && !_.IsDeleted)
                .AnyAsync();
        }

        public async Task<List<Mail>> GetThreadAsync(int threadId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Id == threadId || _.ThreadId == threadId)
                .OrderBy(_ => _.CreatedAt)
                .ProjectToType<Mail>()
                .ToListAsync();
        }
    }
}
