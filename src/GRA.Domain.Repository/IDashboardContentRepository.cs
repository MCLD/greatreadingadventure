using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDashboardContentRepository : IRepository<DashboardContent>
    {
        Task<IEnumerable<DashboardContent>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
        Task<DashboardContent> GetCurrentAsync(int siteId);
    }
}
