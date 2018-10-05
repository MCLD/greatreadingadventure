using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsDatesRepository : IRepository<PsDates>
    {
        Task<PsDates> GetBySiteIdAsync(int siteId);
    }
}
