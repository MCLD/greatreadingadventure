using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsPerformerScheduleRepository
        : AuditingRepository<Model.PsPerformerSchedule, Domain.Model.PsPerformerSchedule>, IPsPerformerScheduleRepository
    {
        public PsPerformerScheduleRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerScheduleRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IList<PsPerformerSchedule>> GetByPerformerIdAsync(int performerId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.PerformerId == performerId)
                .ProjectTo<PsPerformerSchedule>()
                .ToListAsync();
        }

        public async Task SetPerformerScheduleAsync(int performerId,
            List<PsPerformerSchedule> schedule)
        {
            var currentSchedule = DbSet.Where(_ => _.PerformerId == performerId);
            DbSet.RemoveRange(currentSchedule);

            var newSchedule = _mapper
                .Map<List<PsPerformerSchedule>, List<Model.PsPerformerSchedule>>(schedule);
            await DbSet.AddRangeAsync(newSchedule);

            await _context.SaveChangesAsync();
        }
    }
}
