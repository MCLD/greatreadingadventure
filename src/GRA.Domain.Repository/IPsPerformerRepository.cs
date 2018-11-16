using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerRepository : IRepository<PsPerformer>
    {
        Task<PsPerformer> GetByUserIdAsync(int userId);
        Task<DataWithCount<ICollection<PsPerformer>>> PageAsync(BaseFilter filter);
        Task<List<int>> GetIndexListAsync();
        Task<ICollection<Branch>> GetPerformerBranchesAsync(int performerId);
        Task AddPerformerBranchListAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchListAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchesAsync(int performerId);
    }
}
