using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<ICollection<Location>> GetAll(int siteId);
        Task<int> CountAsync(Filter filter);
        Task<ICollection<Location>> PageAsync(Filter filter);
        Task<bool> ValidateAsync(int locationId, int siteId);
    }
}
