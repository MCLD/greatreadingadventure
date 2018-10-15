using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerRepository : IRepository<PsPerformer>
    {
        Task<PsPerformer> GetByUserIdAsync(int userId);
        Task AddPerformerBranchesAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchesAync(int performerId, List<int> branchIds);
    }
}
