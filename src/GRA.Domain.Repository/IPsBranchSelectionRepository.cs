using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsBranchSelectionRepository : IRepository<PsBranchSelection>
    {
        Task<int> GetCountByKitIdAsync(int kitId);
        Task<ICollection<PsBranchSelection>> GetByKitIdAsync(int kitId);
        Task<int> GetCountByPerformerIdAsync(int performerId);
        Task<ICollection<PsBranchSelection>> GetByPerformerIdAsync(int performerId, 
            DateTime? date = null);
        Task<PsBranchSelection> GetByCodeAsync(string secretCode);
        Task<bool> BranchAgeGroupAlreadySelectedAsync(int ageGroupId, int branchSelectionId);
    }
}
