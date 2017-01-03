using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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

        public async Task<IEnumerable<DrawingCriterion>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.Branch)
                    .Where(_ => _.SiteId == siteId)
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<DrawingCriterion>()
                    .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .CountAsync();
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
                    .Where(_ => _.IsDeleted == false);

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
                    pointUsers = pointSum
                        .Where(_ => _.Total >= criterion.PointsMinimum)
                        .Select(_ => _.UserId);
                }
                if (criterion.PointsMaximum != null)
                {
                    var maxUsers = pointSum
                        .Where(_ => _.Total <= criterion.PointsMaximum)
                        .Select(_ => _.UserId);
                    if (pointUsers == null)
                    {
                        pointUsers = maxUsers;
                    }
                    else
                    {
                        pointUsers = pointUsers.Where(_ => maxUsers.Contains(_));
                    }
                }
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
