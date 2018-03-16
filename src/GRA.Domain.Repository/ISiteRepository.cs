using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IRepository<Site>
    {
        Task<IEnumerable<Site>> GetAllAsync();
        Task<DataWithCount<IEnumerable<Site>>> PageAsync(BaseFilter filter);
    }
}
