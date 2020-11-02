using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IUserLogRepository : IRepository<Model.UserLog>
    {
        Task<DataWithCount<ICollection<UserLog>>> GetPaginatedHistoryAsync(UserLogFilter filter);
        Task<long> CompletedChallengeCountAsync(ReportCriterion request, int? challengeId = null);
        Task<long> PointsEarnedTotalAsync(ReportCriterion request);
        Task<Dictionary<string, long>> ActivityEarningsTotalAsync(ReportCriterion request);
        Task<long> GetEarningsOverPeriodAsync(int userId, ReportCriterion criterion);
        Task<long> GetEarningsUpToDateAsync(int userId, DateTime endDate);
        Task<long> EarnedBadgeCountAsync(ReportCriterion request, int? badgeId = null);
        Task<long> TranslationEarningsAsync(ReportCriterion request,
            ICollection<int?> translationIds);
        Task<ICollection<int>> UserIdsEarnedBadgeAsync(int badgeId, ReportCriterion criterion);
        Task<ICollection<int>> UserIdsCompletedChallengesAsync(int challengeId, ReportCriterion criterion);
        Task<int> GetActivityEarnedForUserAsync(int userId);
        Task<bool> PointTranslationHasBeenUsedAsync(int translationId);
    }
}
