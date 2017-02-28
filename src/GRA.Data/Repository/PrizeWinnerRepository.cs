using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

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
    }
}
