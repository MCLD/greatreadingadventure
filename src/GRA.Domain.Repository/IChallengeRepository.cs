using GRA.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<int> GetChallengeCountAsync();
        Task<ICollection<ChallengeTask>> GetChallengeTasksAsync(int challengeId);
    }
}
