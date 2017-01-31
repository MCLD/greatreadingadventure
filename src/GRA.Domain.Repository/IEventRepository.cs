using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<int> CountAsync(int siteId,
            Filter filter = default(Filter),
            string search = default(string),
            bool activeOnly = true);

        Task<ICollection<Event>> PageAsync(int siteId,
            int skip, 
            int take, 
            Filter filter = default(Filter), 
            string search = default(string), 
            bool activeOnly = true);
    }
}
