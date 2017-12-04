using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IPageRepository : IRepository<Page>
    {
        Task<Page> GetByStubAsync(int siteId, string pageStub);
        Task<IEnumerable<Page>> PageAllAsync(int siteId, int skip, int take);
        Task<int> GetCountAsync(int siteId);
        Task<IEnumerable<Page>> GetAreaPagesAsync(int siteId, bool navPages);
    }
}