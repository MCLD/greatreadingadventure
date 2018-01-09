using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IChallengeGroupRepository : IRepository<ChallengeGroup>
    {
        Task<ChallengeGroup> GetActiveByIdAsync(int id);
        Task<ChallengeGroup> GetActiveByStubAsync(int siteId, string stub);
        Task<List<ChallengeGroup>> GetByChallengeId(int siteId, int challengeId);
        Task<int> CountAsync(ChallengeGroupFilter filter);
        Task<IEnumerable<ChallengeGroup>> PageAsync(ChallengeGroupFilter filter);
        Task<ChallengeGroup> AddSaveAsync(int userId, ChallengeGroup challengeGroup,
            IEnumerable<int> challengeIds);
        Task<ChallengeGroup> UpdateSaveAsync(int userId, ChallengeGroup challengeGroup,
            IEnumerable<int> challengesToAdd, IEnumerable<int> challengesToRemove);
        Task<bool> StubInUseAsync(int siteId, string stub);
    }
}
