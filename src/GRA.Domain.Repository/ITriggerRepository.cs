using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ITriggerRepository : IRepository<Trigger>
    {
        Task<ICollection<Trigger>> PageAsync(Filter filter);
        Task<int> CountAsync(Filter filter);
    }
}
