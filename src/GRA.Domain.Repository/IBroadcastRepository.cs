using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IBroadcastRepository : IRepository<Broadcast>
    {
        Task<ICollection<Broadcast>> PageAsync(BroadcastFilter filter);
        Task<int> CountAsync(BroadcastFilter filter);
        Task<IEnumerable<Broadcast>> GetNewBroadcastsAsync(int siteid, DateTime? lastBroadcast);
    }
}
