using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IRepository<Site>
    {
        Task<IEnumerable<Site>> PageAllAsync(int skip, int take);
        Task<IEnumerable<Site>> GetAllAsync();
    }
}
