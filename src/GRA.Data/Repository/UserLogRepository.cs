using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System;

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
                    if(translation.TranslationDescriptionPastTense.Contains("{0}"))
                    {
                        userLog.Description = string.Format(
                            translation.TranslationDescriptionPastTense,
                            userLog.ActivityEarned);
                    }
                    else
                    {
                        userLog.Description = translation.TranslationDescriptionPastTense;
                    }
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

        public async Task<int> CompletedChallengeCountAsync(
            int siteId,
            DateTime? startDate = default(DateTime?),
            DateTime? endDate = default(DateTime?))
        {
            var challengeCount = DbSet
                .AsNoTracking()
                .Where(_ => _.ChallengeId != null);

            if(startDate != null)
            {
                challengeCount = challengeCount
                    .Where(_ => _.CreatedAt >= startDate);
            }

            if(endDate != null)
            {
                challengeCount = challengeCount
                    .Where(_ => _.CreatedAt <= endDate);
            }

            return await challengeCount.CountAsync();
        }

        public async Task<int> PointsEarnedTotalAsync(int siteId, 
            DateTime? startDate = default(DateTime?), 
            DateTime? endDate = default(DateTime?))
        {
            var pointCount = DbSet
                .AsNoTracking();

            if(startDate != null)
            {
                pointCount = pointCount
                    .Where(_ => _.CreatedAt >= startDate);
            }

            if(endDate != null)
            {
                pointCount = pointCount
                    .Where(_ => _.CreatedAt <= endDate);
            }

            return await pointCount.SumAsync(_ => _.PointsEarned);
        }

        public async Task<Dictionary<string, int>> ActivityEarningsTotalAsync(int siteId,
            DateTime? startDate = default(DateTime?),
            DateTime? endDate = default(DateTime?))
        {
            var pointTranslations = await _context.PointTranslations
                .AsNoTracking()
                .ToDictionaryAsync(_ => _.Id, _ => _.ActivityDescription);

            var activityTotals = await DbSet
                .AsNoTracking()
                .Where(_ => _.PointTranslationId > 0)
                .GroupBy(_ => _.PointTranslationId)
                .Select(_ => new {
                    PointTranslationId = _.Key,
                    ActivityTotal = _.Sum(ae => ae.ActivityEarned)
                })
                .ToListAsync();

            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach(var activityTotal in activityTotals)
            {
                result.Add(
                    pointTranslations[activityTotal.PointTranslationId],
                    activityTotal.ActivityTotal ?? 0);
            }

            return result;
        }
    }
}
