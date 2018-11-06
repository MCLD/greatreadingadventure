using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerScheduleRepository : IRepository<PsPerformerSchedule>
    {
        Task<IList<PsPerformerSchedule>> GetByPerformerIdAsync(int performerId);
        Task SetPerformerScheduleAsync(int performerId, List<PsPerformerSchedule> schedule);
    }
}
