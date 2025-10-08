using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PrizeWinnerRepository
        : AuditingRepository<Model.PrizeWinner, PrizeWinner>, IPrizeWinnerRepository
    {
        public PrizeWinnerRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PrizeWinnerRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountByWinnerIdAsync(PrizeFilter filter)
        {
            return await ApplyFilters(filter)
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<int> GetBranchPrizeRedemptionCountAsync(int branchId,
            IEnumerable<int> triggerIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.RedeemedByBranch == branchId && _.TriggerId.HasValue
                    && triggerIds.Contains(_.TriggerId.Value))
                .CountAsync();
        }

        public async Task<List<PrizeCount>> GetHouseholdUnredeemedPrizesAsync(int headId)
        {
            var householdMemberIds = _context.Users
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && (_.Id == headId || _.HouseholdHeadUserId == headId))
                .Select(_ => _.Id);

            var prizeWinnerList = DbSet.AsNoTracking()
                .Where(_ => !_.RedeemedAt.HasValue && householdMemberIds.Contains(_.UserId));

            var prizeGroups = await (from prize in prizeWinnerList
                                     group prize by new { prize.TriggerId, prize.DrawingId } into prizeGroup
                                     select new
                                     {
                                         Count = prizeGroup.Count(),
                                         prizeGroup.Key.DrawingId,
                                         prizeGroup.Key.TriggerId
                                     })
                       .ToListAsync();

            var prizeList = await (from prize in prizeWinnerList
                                   join triggers in _context.Triggers on prize.TriggerId equals triggers.Id into t
                                   from trigger in t.DefaultIfEmpty()
                                   join drawings in _context.Drawings on prize.DrawingId equals drawings.Id into d
                                   from drawing in d.DefaultIfEmpty()
                                   select new
                                   {
                                       DrawingId = drawing != null ? (int?)drawing.Id : null,
                                       TriggerId = trigger != null ? (int?)trigger.Id : null,
                                       Name = drawing.Name ?? trigger.AwardPrizeName
                                   })
                       .Distinct()
                       .ToListAsync();

            return prizeList.Join(prizeGroups,
                    list => list.DrawingId,
                    group => group.DrawingId,
                    (list, group) => new { list, group })
                .Union(prizeList.Join(prizeGroups,
                    list => list.TriggerId,
                    group => group.TriggerId,
                    (list, group) => new { list, group }))
                .Select(_ => new PrizeCount
                {
                    Count = _.group.Count,
                    Name = _.list.Name,
                    DrawingId = _.group.DrawingId,
                    TriggerId = _.group.TriggerId
                })
                .OrderBy(_ => _.Name)
                .ToList();
        }

        public async Task<PrizeWinner> GetPrizeForVendorCodeAsync(int vendorCodeId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.VendorCodeId == vendorCodeId)
                .ProjectToType<PrizeWinner>()
                .SingleOrDefaultAsync();
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
                          .ProjectToType<PrizeWinner>()
                          .ToListAsync();
        }

        public async Task<int> GetSystemPrizeRedemptionCountAsync(int systemId,
            IEnumerable<int> triggerIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.RedeemedBySystem == systemId && _.TriggerId.HasValue
                    && triggerIds.Contains(_.TriggerId.Value))
                .CountAsync();
        }

        public async Task<PrizeWinner> GetUserDrawingPrizeAsync(int userId, int drawingId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId && _.DrawingId == drawingId)
                .ProjectToType<PrizeWinner>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PrizeWinner>> GetUserPrizesAsync(ReportCriterion criterion)
        {
            var validUsers = _context.Users.AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == criterion.SiteId);

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
                          .ProjectToType<PrizeWinner>()
                          .ToListAsync();
        }

        public async Task<PrizeWinner> GetUserTriggerPrizeAsync(int userId, int triggerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.UserId == userId && _.TriggerId == triggerId)
                .ProjectToType<PrizeWinner>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<PrizeWinner>> GetVendorCodePrizesAsync(int userId)
        {
            return await DbSet
                .Where(_ => _.UserId == userId && _.VendorCodeId != null)
                .AsNoTracking()
                .ProjectToType<PrizeWinner>()
                .ToListAsync();
        }

        public async Task<ICollection<PrizeWinner>> PageByWinnerAsync(PrizeFilter filter)
        {
            return await ApplyFilters(filter)
                .AsNoTracking()
                .OrderBy(_ => _.RedeemedAt.HasValue)
                .ThenByDescending(_ => _.RedeemedAt.Value)
                .ApplyPagination(filter)
                .ProjectToType<PrizeWinner>()
                .ToListAsync();
        }

        private IQueryable<Model.PrizeWinner> ApplyFilters(PrizeFilter filter)
        {
            var prizeWinners = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (!filter.IncludeDrawings)
            {
                prizeWinners = prizeWinners.Where(_ => _.DrawingId == null);
            }

            if (filter.IsRedeemed.HasValue)
            {
                prizeWinners = prizeWinners
                    .Where(_ => _.RedeemedAt.HasValue == filter.IsRedeemed.Value);
            }

            if (filter.UserIds?.Count > 0)
            {
                prizeWinners = prizeWinners
                    .Where(_ => filter.UserIds.Contains(_.UserId));
            }

            return prizeWinners;
        }
    }
}
