using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPageHeaderRepository : IRepository<Model.PageHeader>
    {
        Task<DataWithCount<IEnumerable<PageHeader>>> PageAsync(BaseFilter filter);
        Task<bool> StubExistsAsync(int siteId, string stub);
    }
}
