using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DynamicAvatarColorRepository : AuditingRepository<Model.DynamicAvatarColor, DynamicAvatarColor>,
        IDynamicAvatarColorRepository
    {
        public DynamicAvatarColorRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DynamicAvatarColorRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
