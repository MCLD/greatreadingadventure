using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class PrizeWinnerRepository
        : AuditingRepository<Model.PrizeWinner, PrizeWinner>, IPrizeWinnerRepository
    {
        public PrizeWinnerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PrizeWinnerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountByWinningUserId(int siteId, int userId, bool? redeemed = null)
        {
            var prizeWinners = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.UserId == userId);
            if (redeemed.HasValue)
            {
                prizeWinners = prizeWinners.Where(_ => _.RedeemedAt.HasValue == redeemed.Value);
            }

            return await prizeWinners.CountAsync();
        }

        public async Task<ICollection<PrizeWinner>> PageByWinnerAsync(int siteId,
            int userId,
            int skip,
            int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId && _.UserId == userId)
                .OrderBy(_ => _.RedeemedAt.HasValue)
                .ThenByDescending(_ => _.RedeemedAt.Value)
                .Skip(skip)
                .Take(take)
                .ProjectTo<PrizeWinner>()
                .ToListAsync();
        }

        public async Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId && _.TriggerId == triggerId)
                .ProjectTo<PrizeWinner>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PrizeWinner>> GetRedemptionsAsync(ReportCriterion criterion)
        {
            // Includes deleted users and users from other sites
            var validUsers = _context.Users.AsNoTracking();

            if (criterion.BranchId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.BranchId == criterion.BranchId.Value);
            }
            else if (criterion.SystemId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.SystemId == criterion.SystemId.Value);
            }

            return await (from prizes in DbSet.AsNoTracking().Where(_ => _.RedeemedAt.HasValue
                            && _.SiteId == criterion.SiteId)
                          join users in validUsers
                          on prizes.RedeemedBy equals users.Id
                          select prizes)
                          .ProjectTo<PrizeWinner>()
                          .ToListAsync();
        }

        public async Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion)
        {
            var validUsers = _context.Users.AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == criterion.SiteId);

            if (criterion.BranchId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.BranchId == criterion.BranchId.Value);
            }
            else if (criterion.SystemId.HasValue)
            {
                validUsers = validUsers.Where(_ => _.SystemId == criterion.SystemId.Value);
            }

            return await (from prizes in DbSet.Where(_ => _.SiteId == criterion.SiteId)
                          join users in validUsers
                          on prizes.UserId equals users.Id
                          select prizes)
                          .ProjectTo<PrizeWinner>()
                          .ToListAsync();
        }
    }
}
