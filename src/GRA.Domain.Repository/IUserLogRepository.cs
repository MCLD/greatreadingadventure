using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IUserLogRepository : IRepository<Model.UserLog>
    {
        Task<IEnumerable<Model.UserLog>> PageHistoryAsync(int userId, int skip, int take);
        Task<int> GetHistoryItemCountAsync(int userId);
        Task<int> CompletedChallengeCountAsync(
            int siteId,
            DateTime? startDate = default(DateTime?),
            DateTime? endDate = default(DateTime?));
        Task<int> PointsEarnedTotalAsync(
            int siteId,
            DateTime? startDate = default(DateTime?),
            DateTime? endDate = default(DateTime?));
        Task<Dictionary<string, int>> ActivityEarningsTotalAsync(int siteId,
            DateTime? startDate = default(DateTime?),
            DateTime? endDate = default(DateTime?));
    }
}
