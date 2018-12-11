using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsKitRepository : IRepository<PsKit>
    {
        Task<ICollection<PsKit>> GetAllAsync();
        Task<DataWithCount<ICollection<PsKit>>> PageAsync(BaseFilter filter);
        Task<List<int>> GetIndexListAsync();
        Task<ICollection<PsAgeGroup>> GetKitAgeGroupsAsync(int kitId);
        Task AddKitAgeGroupsAsync(int kitIdId, List<int> ageGroupIds);
        Task RemoveKitAgeGroupsAsync(int kitIdId, List<int> ageGroupIds);
        Task<bool> IsValidAgeGroupAsync(int kitId, int ageGroupId);
    }
}
