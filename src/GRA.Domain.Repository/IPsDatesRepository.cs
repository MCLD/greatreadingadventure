using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsSettingsRepository : IRepository<PsSettings>
    {
        Task<PsSettings> GetBySiteIdAsync(int siteId);
        Task<ICollection<int>> GetExcludedBranchIdsAsync();
        Task<ICollection<Branch>> GetNonExcludedSystemBranchesAsync(int systemId,
            int? prioritizeBranchId = null);
        Task<Branch> GetNonExcludedBranchAsync(int branchId);
        Task AddBranchExclusionsAsync(List<int> branchIds);
        Task RemoveBranchExclusionsAsync(List<int> branchIds);
    }
}
