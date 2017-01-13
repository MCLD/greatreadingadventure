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
        Task<int> CompletedChallengeCountAsync(StatusSummary request);
        Task<int> PointsEarnedTotalAsync(StatusSummary request);
        Task<Dictionary<string, int>> ActivityEarningsTotalAsync(StatusSummary request);
        Task<int> EarnedBadgeCountAsync(StatusSummary request);
    }
}
