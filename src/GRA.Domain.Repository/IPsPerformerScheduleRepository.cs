using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.Domain.Repository
{
    public interface IPsPerformerScheduleRepository : IRepository<PsPerformerSchedule>
    {
        Task<List<PsPerformerSchedule>> GetByPerformerIdAsync(int performerId);
        Task<PsPerformerSchedule> GetPerformerDateScheduleAsync(int performerId, DateTime date);
        Task SetPerformerScheduleAsync(int performerId, List<PsPerformerSchedule> schedule);
        Task RemovePerformerScheduleAsync(int performerId);
    }
}
