using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface INewsPostRepository : IRepository<NewsPost>
    {
        Task<bool> AnyPublishedPostsAsync(int siteId);

        Task<NewsPost> GetByIdAsync(int id, bool getBorderingIds);

        Task<int> GetLatestActiveIdAsync(BaseFilter filter);

        Task<DataWithCount<IEnumerable<NewsPost>>> PageAsync(NewsFilter filter);
    }
}
