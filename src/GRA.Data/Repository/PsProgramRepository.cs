using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsProgramRepository
        : AuditingRepository<Model.PsProgram, Domain.Model.PsProgram>, IPsProgramRepository
    {
        public PsProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
