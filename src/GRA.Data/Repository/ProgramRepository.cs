using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ProgramRepository
        : AuditingRepository<Model.Program, Domain.Model.Program>, IProgramRepository
    {
        public ProgramRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ProgramRepository> logger): base(repositoryFacade, logger)
        {
        }
    }
}
