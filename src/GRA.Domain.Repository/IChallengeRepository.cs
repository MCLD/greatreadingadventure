using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<int> GetChallengeCountAsync(int siteId, string search = null);
        Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId, int? userId);
        Task<Challenge> GetByIdAsync(int id, int? userId = null);
        Task<IEnumerable<Challenge>> PageAllAsync(
            int siteId, 
            int skip, 
            int take, 
            string search = null);
        Task UpdateUserChallengeTask(int userId, IEnumerable<ChallengeTask> challengeTasks);
    }
}