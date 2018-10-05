using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsBlackoutDateRepository
        : AuditingRepository<Model.PsBlackoutDate, Domain.Model.PsBlackoutDate>, IPsBlackoutDateRepository
    {
        public PsBlackoutDateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsBlackoutDateRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
