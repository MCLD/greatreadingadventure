using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsPerformerImageRepository
        : AuditingRepository<Model.PsPerformerImage, Domain.Model.PsPerformerImage>, IPsPerformerImageRepository
    {
        public PsPerformerImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsPerformerImageRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
