using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<IEnumerable<Branch>> GetAllAsync(int siteId);
        Task<IEnumerable<Branch>> GetBySystemAsync(int systemId);
        Task<ICollection<Branch>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<bool> IsInUseAsync(int branchId);

        Task<bool> ValidateAsync(int branchId, int systemId);
        Task<bool> ValidateBySiteAsync(int branchId, int siteId);
    }
}
