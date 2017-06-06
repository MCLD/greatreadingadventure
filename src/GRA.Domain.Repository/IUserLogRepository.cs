using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserLogRepository : IRepository<Model.UserLog>
    {
        Task<IEnumerable<Model.UserLog>> PageHistoryAsync(int userId, int skip, int take);
        Task<int> GetHistoryItemCountAsync(int userId);
        Task<long> CompletedChallengeCountAsync(ReportCriterion request);
        Task<long> PointsEarnedTotalAsync(ReportCriterion request);
        Task<Dictionary<string, long>> ActivityEarningsTotalAsync(ReportCriterion request);
        Task<long> EarnedBadgeCountAsync(ReportCriterion request);
    }
}
