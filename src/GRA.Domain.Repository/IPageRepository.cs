using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IPageRepository : IRepository<Page>
    {
        Task<Page> GetByStubAsync(int siteId, string pageStub);
    }
}