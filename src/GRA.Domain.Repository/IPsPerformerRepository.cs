using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerRepository : IRepository<PsPerformer>
    {
        Task<PsPerformer> GetByIdAsync(int id, bool onlyApproved = false);
        Task<PsPerformer> GetByUserIdAsync(int userId);
        Task<DataWithCount<ICollection<PsPerformer>>> PageAsync(PerformerSchedulingFilter filter);
        Task<List<int>> GetIndexListAsync(bool onlyApproved = false);
        Task<ICollection<PsAgeGroup>> GetPerformerAgeGroupsAsync(int performerId);
        Task<ICollection<Branch>> GetPerformerBranchesAsync(int performerId);
        Task<ICollection<int>> GetPerformerBranchIdsAsync(int performerId, int? systemId = null);
        Task<bool> GetPerformerSystemAvailability(int performerId, int systemId);
        Task AddPerformerBranchListAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchListAsync(int performerId, List<int> branchIds);
        Task RemovePerformerBranchesAsync(int performerId);
    }
}
