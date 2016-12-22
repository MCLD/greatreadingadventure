using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> GetCountAsync(int siteId);
        Task<IEnumerable<Category>> PageAllAsync(int siteId, int skip, int take);
    }
}