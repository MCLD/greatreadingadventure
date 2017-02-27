using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<int> GetChallengeCountAsync(
            int siteId, 
            string search = default(string), 
            string filterBy = default(string),
            int? filterId = null,
            int? pendingFor = null);
        Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId, int? userId);
        new Task<Challenge> GetByIdAsync(int id);
        Task<Challenge> GetActiveByIdAsync(int id, int? userId = default(int));
        Task<ICollection<Challenge>> PageAllAsync(
            int siteId,
            int skip,
            int take,
            string search = default(string),
            string filterBy = default(string),
            int? filterId = null,
            int? pendingFor = null);
        Task<DataWithCount<IEnumerable<int>>> PageIdsAsync(
            int siteId,
            int skip,
            int take,
            int userId,
            string search = default(string));
        Task<IEnumerable<ChallengeTaskUpdateStatus>>
            UpdateUserChallengeTasksAsync(int userId, IEnumerable<ChallengeTask> challengeTasks);
        Task UpdateUserChallengeTaskAsync(
            int userId,
            int challengeTaskId,
            int userLogId,
            int? bookId);
        Task<ActivityLogResult> GetUserChallengeTaskResultAsync(int userId, int challengeTaskId);
        Task SetValidationAsync(int userId, int challengeId, bool valid);
        Task<bool> HasDependentsAsync(int challengeId);
    }
}