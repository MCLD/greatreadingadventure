using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<int> CountAsync(Filter filter);
        Task<ICollection<Event>> PageAsync(Filter filter);
        Task<bool> LocationInUse(int siteId, int locationId);
    }
}
