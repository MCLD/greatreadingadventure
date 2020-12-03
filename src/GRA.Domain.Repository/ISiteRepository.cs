using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IRepository<Site>
    {
        Task<IEnumerable<Site>> GetAllAsync();
        Task<DataWithCount<IEnumerable<Site>>> PageAsync(BaseFilter filter);
    }
}
