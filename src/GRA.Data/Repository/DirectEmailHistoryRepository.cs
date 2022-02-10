using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DirectEmailHistoryRepository
        : AuditingRepository<Model.DirectEmailHistory, Domain.Model.DirectEmailHistory>,
        IDirectEmailHistoryRepository
    {
        internal DirectEmailHistoryRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<IDirectEmailHistoryRepository> logger)
            : base(repositoryFacade, logger)
        {
        }
    }
}
