using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsAgeGroupRepository : IRepository<PsAgeGroup>
    {
        Task<ICollection<PsAgeGroup>> GetAllAsync();
        Task<DataWithCount<ICollection<PsAgeGroup>>> PageAsync(BaseFilter filter);
        Task<bool> BranchHasBackToBackAsync(int ageGroupId, int branchId);
    }
}
