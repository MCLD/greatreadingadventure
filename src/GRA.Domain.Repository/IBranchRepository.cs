using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<int> CountAsync(BaseFilter filter);

        Task<IEnumerable<Branch>> GetAllAsync(int siteId, bool requireGeolocation = false);

        Task<IEnumerable<Branch>> GetBySystemAsync(int systemId);

        Task<int> IsInUseAsync(int branchId);

        Task<ICollection<Branch>> PageAsync(BaseFilter filter);

        Task UpdateCreatedByAsync(int userId, int branchId);

        Task<bool> ValidateAsync(int branchId, int systemId);

        Task<bool> ValidateBySiteAsync(int branchId, int siteId);
    }
}
