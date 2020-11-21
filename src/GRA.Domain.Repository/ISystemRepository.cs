using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ISystemRepository : IRepository<Model.System>
    {
        Task<IEnumerable<Model.System>> GetAllAsync(int siteId);
        Task<ICollection<Model.System>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<bool> IsInUseAsync(int systemId);
        Task<bool> ValidateAsync(int systemId, int siteId);
        Task UpdateCreatedByAsync(int userId, int systemId);
    }
}
