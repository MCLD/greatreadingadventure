using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<Event>> PageAsync(BaseFilter filter);
        Task<bool> LocationInUse(int siteId, int locationId);
    }
}
