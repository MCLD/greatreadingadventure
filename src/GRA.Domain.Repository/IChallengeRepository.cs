using GRA.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<int> GetChallengeCountAsync();
        Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId);
        Task<IEnumerable<Challenge>> PageAllAsync(int siteId, int skip, int take);
    }
}
