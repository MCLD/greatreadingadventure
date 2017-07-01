using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DrawingCriterionRepository
        : AuditingRepository<Model.DrawingCriterion, Domain.Model.DrawingCriterion>,
        IDrawingCriterionRepository
    {
        public DrawingCriterionRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingCriterionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<DrawingCriterion>> PageAllAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ThenBy(_ => _.Id)
                .ApplyPagination(filter)
                .ProjectTo<DrawingCriterion>()
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        private IQueryable<Model.DrawingCriterion> ApplyFilters(BaseFilter filter)
        {
            var criterionList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Any() == true)
            {
                criterionList = criterionList
                    .Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }

            if (filter.BranchIds?.Any() == true)
            {
                criterionList = criterionList
                    .Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }

            if (filter.UserIds?.Any() == true)
            {
                criterionList = criterionList.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ProgramIds?.Any() == true)
            {
                criterionList = criterionList
                    .Where(_ => filter.ProgramIds.Any(p => p == _.ProgramId));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                criterionList = criterionList.Where(_ => _.Name.Contains(filter.Search))
                    .Union(criterionList.Where(_ => _.System.Name.Contains(filter.Search)))
                    .Union(criterionList.Where(_ => _.Branch.Name.Contains(filter.Search)));
            }

            return criterionList;
        }

        public async Task<int> GetEligibleUserCountAsync(int criterionId)
        {
            var criterion = await GetByIdAsync(criterionId);
            return await ApplyCriterion(criterion).CountAsync();
        }

        public async Task<ICollection<int>> GetEligibleUserIdsAsync(int criterionId)
        {
            var criterion = await GetByIdAsync(criterionId);
            return await ApplyCriterion(criterion).ToListAsync();
        }

        private IQueryable<int> ApplyCriterion(DrawingCriterion criterion)
        {
            var users = _context.Users
                .AsNoTracking()
                .Where(_ => _.IsDeleted == false && _.SiteId == criterion.SiteId);

            if (criterion.ProgramId != null)
            {
                users = users.Where(_ => _.ProgramId == criterion.ProgramId);
            }

            if (criterion.SystemId != null)
            {
                users = users.Where(_ => _.SystemId == criterion.SystemId);
            }

            if (criterion.BranchId != null)
            {
                users = users.Where(_ => _.BranchId == criterion.BranchId);
            }

            if (!criterion.IncludeAdmin)
            {
                users = users.Where(_ => _.IsAdmin == false);
            }

            if (criterion.ExcludePreviousWinners)
            {
                var previousWinners = _context.PrizeWinners.AsNoTracking()
                    .Where(_ => _.DrawingId.HasValue)
                    .Distinct()
                    .Select(_ => _.UserId);

                users = users.Where(_ => !previousWinners.Contains(_.Id));
            }

            var userIds = users.Select(_ => _.Id);
            IQueryable<int> activityUsers = null;
            IQueryable<int> pointUsers = null;

            if (criterion.ActivityAmount != null && criterion.PointTranslationId != null)
            {
                var userLog = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                        && _.ActivityEarned >= criterion.ActivityAmount
                        && _.PointTranslationId == criterion.PointTranslationId
                        && userIds.Contains(_.UserId));

                if (criterion.StartOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt >= criterion.StartOfPeriod);
                }
                if (criterion.EndOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt <= criterion.EndOfPeriod);
                }

                activityUsers = userLog.Select(_ => _.UserId);
            }

            if (criterion.PointsMinimum != null || criterion.PointsMinimum != null)
            {
                IQueryable<int> pointUserStart = activityUsers != null
                    ? activityUsers
                    : userIds;

                var userLog = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => _.IsDeleted == false
                    && pointUserStart.Contains(_.UserId));

                if (criterion.StartOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt >= criterion.StartOfPeriod);
                }

                if (criterion.EndOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt <= criterion.EndOfPeriod);
                }

                var pointSum = userLog.GroupBy(_ => _.UserId)
                    .Select(sum => new
                    {
                        UserId = sum.Key,
                        Total = sum.Sum(_ => _.PointsEarned)
                    });
                
                if (criterion.PointsMinimum != null)
                {
                    pointSum = pointSum.Where(_ => _.Total >= criterion.PointsMinimum);
                }
                if (criterion.PointsMaximum != null)
                {
                    pointSum = pointSum.Where(_ => _.Total <= criterion.PointsMaximum);
                }

                pointUsers = pointSum.Select(_ => _.UserId);
            }

            if (pointUsers != null)
            {
                return pointUsers;
            }
            if (activityUsers != null)
            {
                return activityUsers;
            }
            return userIds;
        }
    }
}
