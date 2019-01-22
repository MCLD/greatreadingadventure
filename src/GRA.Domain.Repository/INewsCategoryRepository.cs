using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface INewsCategoryRepository : IRepository<NewsCategory>
    {
        Task<NewsCategory> GetDefaultCategoryAsync(int siteId);
        Task<ICollection<NewsCategory>> GetAllAsync(int siteId);
        Task<DataWithCount<IEnumerable<NewsCategory>>> PageAsync(BaseFilter filter);
    }
}
