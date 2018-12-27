using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsSettingsRepository : IRepository<PsSettings>
    {
        Task<PsSettings> GetBySiteIdAsync(int siteId);
        Task<DataWithCount<ICollection<Branch>>> PageExcludedBranchesAsync(BaseFilter filter);
        Task<ICollection<int>> GetExcludedBranchIdsAsync();
        Task<ICollection<Branch>> GetNonExcludedSystemBranchesAsync(int systemId,
            int? prioritizeBranchId = null);
        Task<Branch> GetNonExcludedBranchAsync(int branchId);
        Task AddBranchExclusionAsync(int branchId);
        Task RemoveBranchExclusionAsync(int branchId);
    }
}
