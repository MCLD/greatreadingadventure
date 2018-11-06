using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerRepository : IRepository<PsPerformer>
    {
        Task<PsPerformer> GetByUserIdAsync(int userId);
        Task<ICollection<Branch>> GetPerformerBranchesAsync(int performerId);
        Task AddPerformerBranchesAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchesAsync(int performerId, List<int> branchIds);
    }
}
