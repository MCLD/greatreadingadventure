using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsProgramRepository : IRepository<PsProgram>
    {
        Task AddProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);

        Task<bool> AvailableAtBranchAsync(int programId, int branchId);

        Task<PsProgram> GetByIdAsync(int id, bool onlyApproved);

        Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId, bool onlyApproved);

        Task<int> GetCountByPerformerAsync(int performerId, bool onlyApproved);

        Task<List<int>> GetIndexListAsync(int? ageGroupId, bool onlyApproved);

        Task<ICollection<PsAgeGroup>> GetProgramAgeGroupsAsync(int programId);

        Task<int> GetProgramCountAsync();

        Task<bool> IsValidAgeGroupAsync(int programId, int ageGroupId);

        Task<DataWithCount<ICollection<PsProgram>>> PageAsync(PerformerSchedulingFilter filter);

        Task RemoveProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);
    }
}
