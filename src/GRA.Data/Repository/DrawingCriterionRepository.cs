using System;
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
    public class DrawingCriterionRepository
        : AuditingRepository<Model.DrawingCriterion, Domain.Model.DrawingCriterion>,
        IDrawingCriterionRepository
    {
        public DrawingCriterionRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DrawingCriterionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public override async Task<DrawingCriterion> AddSaveAsync(int userId,
            DrawingCriterion domainEntity)
        {
            ArgumentNullException.ThrowIfNull(domainEntity);

            var newCriterion = await base.AddSaveAsync(userId, domainEntity);

            if (domainEntity.ProgramIds != null)
            {
                var criterionProgramList = new List<Model.DrawingCriterionProgram>();
                foreach (var programId in domainEntity.ProgramIds)
                {
                    criterionProgramList.Add(new Model.DrawingCriterionProgram()
                    {
                        DrawingCriterionId = newCriterion.Id,
                        ProgramId = programId
                    });
                }
                await _context.DrawingCriterionPrograms.AddRangeAsync(criterionProgramList);
                await _context.SaveChangesAsync();
            }

            return newCriterion;
        }

        public override async Task<DrawingCriterion> GetByIdAsync(int id)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.Id == id)
                .ProjectToType<DrawingCriterion>()
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetCountAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var results = await ApplyFiltersAsync(filter);
            return await results.CountAsync();
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

        public async Task<IEnumerable<DrawingCriterion>> PageAllAsync(BaseFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var filtered = await ApplyFiltersAsync(filter);
            return await filtered
                .OrderBy(_ => _.Name)
                .ThenBy(_ => _.Id)
                .ApplyPagination(filter)
                .ProjectToType<DrawingCriterion>()
                .ToListAsync();
        }

        public override async Task<DrawingCriterion> UpdateSaveAsync(int userId,
            DrawingCriterion domainEntity)
        {
            ArgumentNullException.ThrowIfNull(domainEntity);

            var updatedCriterion = await base.UpdateSaveAsync(userId, domainEntity);

            var thisCriterionPrograms = _context.DrawingCriterionPrograms.AsNoTracking()
                .Where(_ => _.DrawingCriterionId == updatedCriterion.Id);

            if (domainEntity.ProgramIds != null)
            {
                var programsToAdd = domainEntity.ProgramIds
                    .Except(thisCriterionPrograms.Select(_ => _.ProgramId))
                    .Select(_ => new Model.DrawingCriterionProgram()
                    {
                        DrawingCriterionId = updatedCriterion.Id,
                        ProgramId = _
                    });
                await _context.DrawingCriterionPrograms.AddRangeAsync(programsToAdd);
            }

            var programsToRemove = thisCriterionPrograms
                .Where(_ => !domainEntity.ProgramIds.Contains(_.ProgramId));
            _context.DrawingCriterionPrograms.RemoveRange(programsToRemove);

            await _context.SaveChangesAsync();

            return updatedCriterion;
        }

        private IQueryable<int> ApplyCriterion(DrawingCriterion criterion)
        {
            var users = _context.Users
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == criterion.SiteId);

            if (criterion.ProgramId != null)
            {
                users = users.Where(_ => _.ProgramId == criterion.ProgramId);
            }
            else if (criterion.ProgramIds?.Count > 0)
            {
                users = users.Where(_ => criterion.ProgramIds.Contains(_.ProgramId));
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
                users = users.Where(_ => !_.IsAdmin);
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

            if (criterion.ReadABook)
            {
                var userLog = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => !_.IsDeleted && _.ActivityEarned > 0
                        && _.PointTranslationId.HasValue
                        && userIds.Contains(_.UserId));

                if (criterion.StartOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt >= criterion.StartOfPeriod);
                }
                if (criterion.EndOfPeriod != null)
                {
                    userLog = userLog.Where(_ => _.CreatedAt <= criterion.EndOfPeriod);
                }

                activityUsers = userLog.Select(_ => _.UserId).Distinct();
            }

            if (criterion.PointsMinimum != null || criterion.PointsMinimum != null)
            {
                IQueryable<int> pointUserStart = activityUsers ?? userIds;

                var userLog = _context.UserLogs
                    .AsNoTracking()
                    .Where(_ => !_.IsDeleted && pointUserStart.Contains(_.UserId));

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

            return pointUsers ?? (activityUsers ?? userIds);
        }

        private async Task<IQueryable<Model.DrawingCriterion>> ApplyFiltersAsync(BaseFilter filter)
        {
            var criterionList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.SystemIds?.Count > 0)
            {
                criterionList = criterionList
                    .Where(_ => filter.SystemIds.Contains(_.RelatedSystemId));
            }

            if (filter.BranchIds?.Count > 0)
            {
                criterionList = criterionList
                    .Where(_ => filter.BranchIds.Contains(_.RelatedBranchId));
            }

            if (filter.UserIds?.Count > 0)
            {
                criterionList = criterionList.Where(_ => filter.UserIds.Contains(_.CreatedBy));
            }

            if (filter.ProgramIds?.Count > 0)
            {
                // list of non-null ints to filter by
                var programIds = filter.ProgramIds.Where(_ => _.HasValue).Cast<int>().ToList();

                if (programIds.Count == 0)
                {
                    // filter by no programs
                    criterionList = criterionList
                        .Where(_ => _.CriterionPrograms.Count == 0 && !_.ProgramId.HasValue);
                }
                else
                {
                    // filter by programs in list
                    criterionList = criterionList
                        .Where(_ => filter.ProgramIds.Contains(_.ProgramId)
                            || _.CriterionPrograms
                                .Any(c => filter.ProgramIds.Contains(c.ProgramId)));
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var systemIds = await _context.Systems
                    .Where(_ => _.Name.Contains(filter.Search))
                    .Select(_ => _.Id)
                    .ToListAsync();

                var branchIds = await _context.Branches
                    .Where(_ => _.Name.Contains(filter.Search))
                    .Select(_ => _.Id)
                    .ToListAsync();

                criterionList = criterionList.Where(_ => _.Name.Contains(filter.Search)
                    || (_.SystemId.HasValue && systemIds.Contains(_.SystemId.Value))
                    || (_.BranchId.HasValue && branchIds.Contains(_.BranchId.Value)));
            }

            return criterionList;
        }
    }
}
