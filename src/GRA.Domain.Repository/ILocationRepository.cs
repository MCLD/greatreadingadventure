using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<ICollection<Location>> GetAll(int siteId);
    }
}
