using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IUserLogRepository : IRepository<Model.UserLog>
    {
        Task<Dictionary<string, long>> ActivityEarningsTotalAsync(ReportCriterion request);

        Task<(long earned, long earnedAchiever)>
            CompletedChallengeCountAsync(ReportCriterion request);

        Task<(long earned, long earnedAchiever)>
            CompletedChallengeCountAsync(ReportCriterion request, int? challengeId);

        Task<(long earned, long earnedAchiever)> EarnedBadgeCountAsync(ReportCriterion request);

        Task<(long earned, long earnedAchiever)>
            EarnedBadgeCountAsync(ReportCriterion request, int? badgeId);

        Task<int> GetActivityEarnedForUserAsync(int userId);

        Task<long> GetEarningsOverPeriodAsync(int userId, ReportCriterion criterion);

        Task<long> GetEarningsUpToDateAsync(int userId, DateTime endDate);

        Task<DataWithCount<ICollection<UserLog>>> GetPaginatedHistoryAsync(UserLogFilter filter);

        Task<int> GetSiteActivityEarnedAsync(int siteId);

        Task<long> PointsEarnedTotalAsync(ReportCriterion request);

        Task<bool> PointTranslationHasBeenUsedAsync(int translationId);

        Task<long> TranslationEarningsAsync(ReportCriterion request,
                    ICollection<int?> translationIds);

        Task<ICollection<int>> UserIdsCompletedChallengesAsync(int challengeId, ReportCriterion criterion);

        Task<ICollection<int>> UserIdsEarnedBadgeAsync(int badgeId, ReportCriterion criterion);
    }
}
