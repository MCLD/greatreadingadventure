using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;

namespace GRA.Data.Repository
{
    public class StaticAvatarRepository
        : AuditingRepository<Model.StaticAvatar, Domain.Model.StaticAvatar>,
        IStaticAvatarRepository
    {
        public StaticAvatarRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<StaticAvatarRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
