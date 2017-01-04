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
    public class UserLogRepository
        : AuditingRepository<Model.UserLog, Domain.Model.UserLog>, IUserLogRepository
    {
        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<UserLog>> PageHistoryAsync(int userId, int skip, int take)
        {
            var userLogs = await DbSet
               .AsNoTracking()
               .Where(_ => _.UserId == userId
                      && _.IsDeleted == false)
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
                       && _.IsDeleted == false)
                .CountAsync();
        }

        public async override Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }

        private async Task<ICollection<int>> GetEligibleUserIds(StatusSummary request)
        {
            if (request.ProgramId != null
               || request.SystemId != null
               || request.BranchId != null)
            {
                var eligibleUsers = _context.Users
                    .AsNoTracking()
                    .Where(_ => _.SiteId == request.SiteId && _.IsDeleted == false);
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

        public async Task<int> CompletedChallengeCountAsync(StatusSummary request)
        {
            var eligibleUserIds = await GetEligibleUserIds(request);

            var challengeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.ChallengeId != null);

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

            return await challengeCount.CountAsync();
        }

        public async Task<int> PointsEarnedTotalAsync(StatusSummary request)
        {
            var eligibleUserIds = await GetEligibleUserIds(request);

            var pointCount = DbSet
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

            return await pointCount.SumAsync(_ => _.PointsEarned);
        }

        public async Task<Dictionary<string, int>> ActivityEarningsTotalAsync(StatusSummary request)
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
                .Where(_ => _.PointTranslationId != null);

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
                    ActivityTotal = _.Sum(ae => ae.ActivityEarned)
                })
                .ToListAsync();

            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach (var earned in earnedTotals)
            {
                int earnedSum = earned.ActivityTotal ?? 0;
                int pointTranslationId = (int)earned.PointTranslationId;

                string description = earnedSum == 1
                    ? translationLookup[pointTranslationId].ActivityDescription
                    : translationLookup[pointTranslationId].ActivityDescriptionPlural;

                result.Add(description, earnedSum);
            }

            return result;
        }
    }
}
