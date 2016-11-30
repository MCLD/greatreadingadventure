using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISystemRepository : IRepository<Model.System>
    {
        Task<IEnumerable<Model.System>> GetAllAsync(int siteId);
    }
}
