using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<IEnumerable<Branch>> GetAllAsync(int siteId);
        Task<IEnumerable<Branch>> GetBySystemAsync(int systemId);
        Task<bool> ValidateAsync(int branchId, int systemId);
        Task<bool> ValidateBySiteAsync(int branchId, int siteId);
    }
}
