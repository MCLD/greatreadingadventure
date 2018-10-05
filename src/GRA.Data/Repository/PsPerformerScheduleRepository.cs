using GRA.Domain.Repository;
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
    }
}
