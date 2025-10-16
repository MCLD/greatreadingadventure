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
    public class UserLogRepository : AuditingRepository<Model.UserLog, UserLog>, IUserLogRepository
    {
        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<Dictionary<string, long>>
            ActivityEarningsTotalAsync(ReportCriterion request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var eligibleUsers = GetEligibleUsers(request);

            // build lookup of point translations
            var translationLookup = await _context.PointTranslations
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id);

            // start out with all line items that have a point translation id
            var earnedFilter = DbSet
                .AsNoTracking()
                .Where(_ => _.PointTranslationId != null
                    && !_.IsDeleted
                    && eligibleUsers.Contains(_.User));

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
            // converting a nullable int to an int64 results in a client-side evaluation error
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

        public async Task<(long earned, long earnedAchiever)>
            CompletedChallengeCountAsync(ReportCriterion request, int? challengeId)
        {
            ArgumentNullException.ThrowIfNull(request);

            var eligibleUsers = GetEligibleUsers(request);

            var challengeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.ChallengeId != null
                    && !_.IsDeleted
                    && eligibleUsers.Contains(_.User));

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

            var earned = await challengeCount.CountAsync();

            var earnedAchiever = request.IncludeAchieverStatus
                ? await challengeCount.CountAsync(_ => _.User.AchievedAt != null)
                : 0;

            return (earned, earnedAchiever);
        }

        public async Task<(long earned, long earnedAchiever)>
            CompletedChallengeCountAsync(ReportCriterion request)
        {
            return await CompletedChallengeCountAsync(request, null);
        }

        public async Task<(long earned, long earnedAchiever)>
            EarnedBadgeCountAsync(ReportCriterion request, int? badgeId)
        {
            ArgumentNullException.ThrowIfNull(request);

            var eligibleUsers = GetEligibleUsers(request);

            var badgeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.BadgeId != null
                    && !_.IsDeleted
                    && eligibleUsers.Contains(_.User));

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

            var earned = await badgeCount.CountAsync();

            var earnedAchiever = request.IncludeAchieverStatus
                ? await badgeCount.CountAsync(_ => _.User.AchievedAt != null)
                : 0;

            return (earned, earnedAchiever);
        }

        public async Task<(long earned, long earnedAchiever)>
            EarnedBadgeCountAsync(ReportCriterion request)
        {
            return await EarnedBadgeCountAsync(request, null);
        }

        public async Task<int> GetActivityEarnedForUserAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId && !_.IsDeleted && _.ActivityEarned.HasValue)
                .SumAsync(_ => _.ActivityEarned.Value);
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

        public async Task<DataWithCount<ICollection<UserLog>>>
            GetPaginatedHistoryAsync(UserLogFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var userLogs = DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted);

            if (filter.UserIds?.Any() == true)
            {
                userLogs = userLogs.Where(_ => filter.UserIds.Contains(_.UserId));
            }

            if (filter.HasBadge.HasValue)
            {
                userLogs = userLogs.Where(_ => _.BadgeId.HasValue == filter.HasBadge);
            }

            if (filter.HasAttachment.HasValue)
            {
                userLogs = userLogs.Where(_ => _.AttachmentId.HasValue == filter.HasAttachment);
            }

            var count = await userLogs.CountAsync();
            var data = await userLogs
                .OrderByDescending(_ => _.CreatedAt)
                .ApplyPagination(filter)
                .ProjectToType<UserLog>()
                .ToListAsync();

            foreach (var userLog in data)
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

                        if (userLog.BookId != null)
                        {
                            var book = _context.Books
                                .AsNoTracking()
                                .Where(_ => _.Id == userLog.BookId)
                                .SingleOrDefault();

                            if (book != null)
                            {
                                userLog.Description += " - "
                                    + book.Title
                                    + (book.Author != null ? " (" + book.Author + ")" : "");
                            }
                        }

                        userLog.Description =
                            userLog.Description.Substring(0, 1).ToUpper()
                                + userLog.Description.Substring(1);
                    }
                }
                if (userLog.BadgeId != null)
                {
                    var badge = _context.Badges
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.BadgeId)
                        .SingleOrDefault();
                    userLog.BadgeFilename = badge.Filename;
                    userLog.BadgeAltText = badge.AltText;
                }
                if (userLog.AttachmentId != null)
                {
                    var attachment = _context.Attachments
                        .AsNoTracking()
                        .Where(_ => _.Id == userLog.AttachmentId)
                        .SingleOrDefault();
                    userLog.AttachmentFilename = attachment.FileName;
                    userLog.AttachmentIsCertificate = attachment.IsCertificate.Value;
                }
            }

            return new DataWithCount<ICollection<UserLog>>
            {
                Count = count,
                Data = data
            };
        }

        public async Task<int> GetSiteActivityEarnedAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && !_.User.IsDeleted && _.User.SiteId == siteId)
                .SumAsync(_ => _.ActivityEarned.Value);
        }

        public async Task<long> PointsEarnedTotalAsync(ReportCriterion request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var eligibleUsers = GetEligibleUsers(request);

            var pointCount = DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && eligibleUsers.Contains(_.User));

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

            long result = 0;
            try
            {
                result = await pointCount.SumAsync(_ => Convert.ToInt64(_.PointsEarned));
            }
            catch (InvalidOperationException)
            {
                // may be using a database provider (SQLite) that doesn't support Convert.ToInt64
                foreach (var points in pointCount.Select(_ => _.PointsEarned))
                {
                    result += points;
                }
            }
            return result;
        }

        public async Task<bool> PointTranslationHasBeenUsedAsync(int translationId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PointTranslationId == translationId)
                .AnyAsync();
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

        public async Task<long> TranslationEarningsAsync(ReportCriterion request,
            ICollection<int?> translationIds)
        {
            ArgumentNullException.ThrowIfNull(request);
            var eligibleUsers = GetEligibleUsers(request);

            var earnedFilter = DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted
                    && _.ActivityEarned != null
                    && eligibleUsers.Contains(_.User)
                    && _.PointTranslationId != null
                    && translationIds.Contains(_.PointTranslationId));

            if (request.MaximumAllowableActivity > 0)
            {
                return await earnedFilter
                    .GroupBy(_ => _.UserId)
                    .Select(_ => _.Sum(s => Convert.ToInt64(s.ActivityEarned.Value)))
                    .Where(_ => _ > 0)
                    .Select(_ => _ > request.MaximumAllowableActivity.Value ? request.MaximumAllowableActivity.Value : _)
                    .SumAsync();
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

            return await earnedFilter.SumAsync(_ => Convert.ToInt64(_.ActivityEarned.Value));
        }

        public async Task<ICollection<int>>
            UserIdsCompletedChallengesAsync(int challengeId, ReportCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);
            return await GetEligibleUserLogs(criterion)
                .Where(_ => _.ChallengeId == challengeId)
                .Select(_ => _.UserId)
                .ToListAsync();
        }

        public async Task<ICollection<int>>
            UserIdsEarnedBadgeAsync(int badgeId, ReportCriterion criterion)
        {
            ArgumentNullException.ThrowIfNull(criterion);
            return await GetEligibleUserLogs(criterion)
                .Where(_ => _.BadgeId == badgeId)
                .Select(_ => _.UserId)
                .ToListAsync();
        }

        private IQueryable<Model.UserLog> GetEligibleUserLogs(ReportCriterion request)
        {
            var eligibleUserLogs = DbSet
                    .AsNoTracking()
                    .Where(_ => _.User.SiteId == request.SiteId
                        && !_.IsDeleted
                        && !_.User.IsDeleted);

            if (request.ProgramId != null)
            {
                eligibleUserLogs = eligibleUserLogs
                    .Where(_ => _.User.ProgramId == request.ProgramId);
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

        private IQueryable<Model.User> GetEligibleUsers(ReportCriterion request)
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

            return eligibleUsers;
        }
    }
}
