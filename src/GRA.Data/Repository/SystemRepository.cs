using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SystemRepository
        : AuditingRepository<Model.System, Domain.Model.System>, ISystemRepository
    {
        public SystemRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<SystemRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
