using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsAgeGroupRepository 
        : AuditingRepository<Model.PsAgeGroup, Domain.Model.PsAgeGroup>, IPsAgeGroupRepository
    {
        public PsAgeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsAgeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
