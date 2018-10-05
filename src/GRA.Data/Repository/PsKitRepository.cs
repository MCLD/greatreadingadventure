using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsKitRepository
        : AuditingRepository<Model.PsKit, Domain.Model.PsKit>, IPsKitRepository
    {
        public PsKitRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsKitRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
