using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<Challenge> GetActiveByIdAsync(int id, int? userId = default(int));

        new Task<Challenge> GetByIdAsync(int id);

        Task<List<Challenge>> GetByIdsAsync(int siteId, IEnumerable<int> ids,
            bool ActiveOnly = false);

        Task<int> GetChallengeCountAsync(ChallengeFilter filter);

        Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId, int? userId);

        Task<IEnumerable<string>> GetNamesAsync(IEnumerable<int> challengeIds);

        Task<ActivityLogResult> GetUserChallengeTaskResultAsync(int userId, int challengeTaskId);

        Task<IEnumerable<int>> GetUserFavoriteChallenges(int userId,
            IEnumerable<int> challengeIds = null);

        Task<bool> HasDependentsAsync(int challengeId);

        Task IncrementPopularity(int challengeId);

        Task<ICollection<Challenge>> PageAllAsync(ChallengeFilter filter);

        Task<DataWithCount<IEnumerable<int>>> PageIdsAsync(ChallengeFilter filter, int userId);

        Task SetValidationAsync(int userId, int challengeId, bool valid);

        Task<Challenge> UpdateSaveAsync(int currentUserId,
            Challenge challenge,
            List<int> categoriesToAdd,
            List<int> categoriesToRemove);

        Task UpdateUserChallengeTaskAsync(
            int userId,
            int challengeTaskId,
            int? userLogId,
            int? bookId);

        Task<IEnumerable<ChallengeTaskUpdateStatus>> UpdateUserChallengeTasksAsync(int userId,
            IEnumerable<ChallengeTask> challengeTasks);

        Task UpdateUserFavoritesAsync(int authUserId,
            int userId,
            IEnumerable<int> favoritesToAdd,
            IEnumerable<int> favoritesToRemove);

        Task<IEnumerable<int>> ValidateChallengeIdsAsync(int siteId, IEnumerable<int> challengeIds);
    }
}
