using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsProgramRepository : IRepository<PsProgram>
    {
        Task<ICollection<PsProgram>> GetByPerformerIdAsync(int performerId);
        Task AddProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);
        Task RemoveProgramAgeGroupsAsync(int programId, List<int> ageGroupIds);
    }
}
