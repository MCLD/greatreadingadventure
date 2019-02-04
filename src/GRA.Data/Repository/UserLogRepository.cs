using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class UserLogRepository : AuditingRepository<Model.UserLog, UserLog>, IUserLogRepository
    {
        public readonly int MaximumAllowableActivity;

        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
            var configuredMaxActivity
                = repositoryFacade.config[ConfigurationKey.MaximumAllowableActivity];
            if (!string.IsNullOrEmpty(configuredMaxActivity))
            {
                if (!int.TryParse(configuredMaxActivity, out MaximumAllowableActivity))
                {
                    _logger.LogError("Could not configure maximum allowable activity: {0} in configuration is not a number",
                        ConfigurationKey.MaximumAllowableActivity);
                }
            }
        }

        public async Task<IEnumerable<UserLog>> PageHistoryAsync(int userId, int skip, int take)
        {
            var userLogs = await DbSet
               .AsNoTracking()
               .Where(_ => _.UserId == userId
                      && !_.IsDeleted)
               .OrderBy(_ => _.CreatedAt)
               .Skip(skip)
               .Take(take)
               .ProjectTo<UserLog>()
               .ToListAsync();

            foreach (var userLog in userLogs)
            {
                if (userLog.ChallengeId != null)
                {
                    var challenge = _context.Challenges
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.ChallengeId)
                        .SingleOrDefault();
                    userLog.Description = $"Completed challenge: {challenge.Name}";
                }
                else
                {
                    // used a point translation
                    var translation = _context.PointTranslations
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.PointTranslationId)
                        .SingleOrDefault();
                    if (translation != null)
                    {
                        if (translation.TranslationDescriptionPastTense.Contains("{0}"))
                        {
                            userLog.Description = string.Format(
                                translation.TranslationDescriptionPastTense,
                                userLog.ActivityEarned);
                            if (userLog.ActivityEarned == 1)
                            {
                                userLog.Description += $" {translation.ActivityDescription}";
                            }
                            else
                            {
                                userLog.Description += $" {translation.ActivityDescriptionPlural}";
                            }
                        }
                        else
                        {
                            userLog.Description = $"{translation.TranslationDescriptionPastTense} {translation.ActivityDescription}";
                        }
                        userLog.Description =
                            userLog.Description.Substring(0, 1).ToUpper()
                            + userLog.Description.Substring(1);
                    }
                }
                if (userLog.BadgeId != null)
                {
                    userLog.BadgeFilename = _context.Badges
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.BadgeId)
                        .SingleOrDefault()
                        .Filename;
                }
            }

            return userLogs;
        }

        public async Task<int> GetHistoryItemCountAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId
                       && !_.IsDeleted)
                .CountAsync();
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

        private async Task<ICollection<int>> GetEligibleUserIds(ReportCriterion request)
        {
            if (request.ProgramId != null
               || request.SystemId != null
               || request.BranchId != null)
            {
                var eligibleUsers = _context.Users
                    .AsNoTracking()
                    .Where(_ => _.SiteId == request.SiteId && !_.IsDeleted);
                if (request.ProgramId != null)
                {
                    eligibleUsers = eligibleUsers.Where(_ => _.ProgramId == request.ProgramId);
                }
                if (request.SystemId != null)
                {
                    eligibleUsers = eligibleUsers.Where(_ => _.SystemId == request.SystemId);
                }
                if (request.BranchId != null)
                {
                    eligibleUsers = eligibleUsers.Where(_ => _.BranchId == request.BranchId);
                }
                return await eligibleUsers.Select(_ => _.Id).ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<long> CompletedChallengeCountAsync(ReportCriterion request,
            int? challengeId = null)
        {
            var eligibleUserIds = await GetEligibleUserIds(request);

            var challengeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.ChallengeId != null
                    && !_.IsDeleted
                    && !_.User.IsDeleted);

            if (eligibleUserIds != null)
            {
                challengeCount = challengeCount.Where(_ => eligibleUserIds.Contains(_.UserId));
            }

            if (request.StartDate != null)
            {
                challengeCount = challengeCount
                    .Where(_ => _.CreatedAt >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                challengeCount = challengeCount
                    .Where(_ => _.CreatedAt <= request.EndDate);
            }

            if (challengeId != null)
            {
                challengeCount = challengeCount
                    .Where(_ => _.ChallengeId == challengeId);
            }

            return await challengeCount.CountAsync();
        }

        public async Task<long> PointsEarnedTotalAsync(ReportCriterion request)
        {
            var eligibleUserIds = await GetEligibleUserIds(request);

            var pointCount = DbSet
                .Where(_ => !_.IsDeleted && !_.User.IsDeleted)
                .AsNoTracking();

            if (eligibleUserIds != null)
            {
                pointCount = pointCount.Where(_ => eligibleUserIds.Contains(_.UserId));
            }

            if (request.StartDate != null)
            {
                pointCount = pointCount
                    .Where(_ => _.CreatedAt >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                pointCount = pointCount
                    .Where(_ => _.CreatedAt <= request.EndDate);
            }

            return await pointCount.SumAsync(_ => Convert.ToInt64(_.PointsEarned));
        }

        public async Task<Dictionary<string, long>> ActivityEarningsTotalAsync(ReportCriterion request)
        {
            // look up user id restrictions
            var eligibleUserIds = await GetEligibleUserIds(request);

            // build lookup of point translations
            var translationLookup = await _context.PointTranslations
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id);

            // start out with all line items that have a point translation id
            var earnedFilter = DbSet
                .AsNoTracking()
                .Where(_ => _.PointTranslationId != null
                    && !_.IsDeleted
                    && !_.User.IsDeleted);

            // filter by users if necessary
            if (eligibleUserIds != null)
            {
                earnedFilter = earnedFilter.Where(_ => eligibleUserIds.Contains(_.UserId));
            }

            if (request.StartDate != null)
            {
                earnedFilter = earnedFilter
                    .Where(_ => _.CreatedAt >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                earnedFilter = earnedFilter
                    .Where(_ => _.CreatedAt <= request.EndDate);
            }

            // group them by point translation id
            var earnedTotals = await earnedFilter
                .GroupBy(_ => _.PointTranslationId)
                .Select(_ => new
                {
                    PointTranslationId = _.Key,
                    ActivityTotal = _.Sum(ae => Convert.ToInt64(ae.ActivityEarned))
                })
                .ToListAsync();

            var result = new Dictionary<string, long>();
            foreach (var earned in earnedTotals)
            {
                long earnedSum = earned.ActivityTotal;
                int pointTranslationId = (int)earned.PointTranslationId;

                string description = translationLookup[pointTranslationId].ActivityDescription;

                if (result.ContainsKey(description))
                {
                    result[description] += earnedSum;
                }
                else
                {
                    result.Add(description, earnedSum);
                }
            }

            var namedResult = new Dictionary<string, long>();
            foreach (var item in result)
            {
                if (item.Value > 1)
                {
                    var name = translationLookup.Values
                        .Where(_ => _.ActivityDescription == item.Key)
                        .Select(_ => _.ActivityDescriptionPlural)
                        .FirstOrDefault();
                    namedResult.Add(name, item.Value);
                }
                else
                {
                    namedResult.Add(item.Key, item.Value);
                }
            }

            return namedResult;
        }

        public async Task<long> GetEarningsOverPeriodAsync(int userId, ReportCriterion criterion)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId
                    && !_.IsDeleted
                    && _.PointsEarned > 0
                    && _.CreatedAt >= criterion.StartDate
                    && _.CreatedAt <= criterion.EndDate)
                .SumAsync(_ => Convert.ToInt64(_.PointsEarned));
        }

        public async Task<long> GetEarningsUpToDateAsync(int userId, DateTime endDate)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId
                    && !_.IsDeleted
                    && _.PointsEarned > 0
                    && _.CreatedAt < endDate)
                .SumAsync(_ => Convert.ToInt64(_.PointsEarned));
        }

        public async Task<long> EarnedBadgeCountAsync(ReportCriterion request,
            int? badgeId = null)
        {
            var eligibleUserIds = await GetEligibleUserIds(request);

            var badgeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.BadgeId != null
                    && !_.IsDeleted
                    && !_.User.IsDeleted);

            if (eligibleUserIds != null)
            {
                badgeCount = badgeCount.Where(_ => eligibleUserIds.Contains(_.UserId));
            }

            if (request.StartDate != null)
            {
                badgeCount = badgeCount
                    .Where(_ => _.CreatedAt >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                badgeCount = badgeCount
                    .Where(_ => _.CreatedAt <= request.EndDate);
            }

            if (badgeId != null)
            {
                badgeCount = badgeCount
                    .Where(_ => _.BadgeId == badgeId);
            }

            return await badgeCount.CountAsync();
        }

        public async Task<long> TranslationEarningsAsync(ReportCriterion request,
            ICollection<int?> translationIds)
        {
            // look up user id restrictions
            var eligibleUserIds = await GetEligibleUserIds(request);

            var earnedFilter = DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.ActivityEarned != null
                    && !_.User.IsDeleted
                    && _.PointTranslationId != null
                    && translationIds.Contains(_.PointTranslationId));

            // filter by users if necessary
            if (eligibleUserIds != null)
            {
                earnedFilter = earnedFilter.Where(_ => eligibleUserIds.Contains(_.UserId));

                if (MaximumAllowableActivity > 0)
                {
                    return await _context.Users
                        .Where(_ => eligibleUserIds.Contains(_.Id))
                        .GroupJoin(earnedFilter, u => u.Id, ul => ul.UserId, (u, ul) => new { User = u, Sum = (long)ul.Sum(_ => _.ActivityEarned) })
                        .Where(_ => _.Sum > 0)
                        .Select(_ => (_.Sum > MaximumAllowableActivity ? _.User.Program.AchieverPointAmount : _.Sum))
                        .SumAsync(_ => _);
                }
            }

            // TODO Fix for reporting over a time period
            /*
            if (request.StartDate != null)
            {
                earnedFilter = earnedFilter
                    .Where(_ => _.CreatedAt >= request.StartDate);
            }

            if (request.EndDate != null)
            {
                earnedFilter = earnedFilter
                    .Where(_ => _.CreatedAt <= request.EndDate);
            }
            */

            return await earnedFilter.SumAsync(_ => Convert.ToInt64(_.ActivityEarned));
        }

        private IQueryable<Model.UserLog> GetEligibleUserLogs(ReportCriterion request)
        {
            if (request.ProgramId != null
               || request.SystemId != null
               || request.BranchId != null)
            {
                var eligibleUserLogs = DbSet
                    .AsNoTracking()
                    .Where(_ => _.User.SiteId == request.SiteId
                        && !_.IsDeleted
                        && !_.User.IsDeleted);

                if (request.ProgramId != null)
                {
                    eligibleUserLogs = eligibleUserLogs.Where(_ => _.User.ProgramId == request.ProgramId);
                }
                if (request.SystemId != null)
                {
                    eligibleUserLogs = eligibleUserLogs.Where(_ => _.User.SystemId == request.SystemId);
                }
                if (request.BranchId != null)
                {
                    eligibleUserLogs = eligibleUserLogs.Where(_ => _.User.BranchId == request.BranchId);
                }
                return eligibleUserLogs;
            }
            else
            {
                return DbSet
                    .AsNoTracking()
                    .Where(_ => _.User.SiteId == request.SiteId
                        && !_.IsDeleted
                        && !_.User.IsDeleted);
            }
        }

        public async Task<ICollection<int>> UserIdsEarnedBadgeAsync(int badgeId, ReportCriterion criterion)
        {
            return await GetEligibleUserLogs(criterion)
                .Where(_ => _.BadgeId == badgeId)
                .Select(_ => _.UserId)
                .ToListAsync();
        }

        public async Task<ICollection<int>> UserIdsCompletedChallengesAsync(int challengeId, ReportCriterion criterion)
        {
            return await GetEligibleUserLogs(criterion)
                .Where(_ => _.ChallengeId == challengeId)
                .Select(_ => _.UserId)
                .ToListAsync();
        }

        public async Task<long> GetActivityEarnedForUserAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId && !_.IsDeleted && _.ActivityEarned.HasValue)
                .SumAsync(_ => Convert.ToInt64(_.ActivityEarned.Value));
        }

        public async Task<bool> PointTranslationHasBeenUsedAsync(int translationId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PointTranslationId == translationId)
                .AnyAsync();
        }
    }
}