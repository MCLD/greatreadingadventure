using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ExitLandingMessagesRepository
    : AuditingRepository<Model.ExitLandingMessageSet, Domain.Model.ExitLandingMessageSet>,
        IExitLandingMessagesRepository
    {
        public ExitLandingMessagesRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<ExitLandingMessagesRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
