using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsProgramRepository : IRepository<PsProgram>
    {
        Task<PsProgram> GetByIdAsync(int id, bool onlyApproved = false);
        Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId);
        Task<DataWithCount<ICollection<PsProgram>>> PageAsync(PerformerSchedulingFilter filter);
        Task<List<int>> GetIndexListAsync(int? ageGroupId = null, bool onlyApproved = false);
        Task<int> GetCountByPerformerAsync(int performerId);
        Task<bool> IsValidAgeGroupAsync(int programId, int ageGroupId);
        Task<ICollection<PsAgeGroup>> GetProgramAgeGroupsAsync(int programId);
        Task AddProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);
        Task RemoveProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);
        Task<bool> AvailableAtBranchAsync(int programId, int branchId);
    }
}
