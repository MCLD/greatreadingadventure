using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IRepository<Model.Site>
    {
        Task<IEnumerable<Model.Site>> PageAllAsync(int skip, int take);
    }
}
