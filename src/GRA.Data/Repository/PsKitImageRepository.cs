using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsKitImageRepository
        : AuditingRepository<Model.PsKitImage, Domain.Model.PsKitImage>, IPsKitImageRepository
    {
        public PsKitImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsKitImageRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
