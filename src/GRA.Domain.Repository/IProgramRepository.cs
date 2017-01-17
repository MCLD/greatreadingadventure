using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IProgramRepository : IRepository<Program>
    {
        Task<IEnumerable<Program>> GetAllAsync(int siteId);
    }
}
