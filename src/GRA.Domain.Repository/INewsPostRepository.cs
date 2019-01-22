using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface INewsPostRepository : IRepository<NewsPost>
    {
        Task<DataWithCount<IEnumerable<NewsPost>>> PageAsync(BaseFilter filter);
    }
}
