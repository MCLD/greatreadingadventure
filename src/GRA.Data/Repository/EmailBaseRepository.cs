using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailBaseRepository
        : AuditingRepository<Model.EmailBase, Domain.Model.EmailBase>,
        IEmailBaseRepository
    {
        internal EmailBaseRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EmailBaseRepository> logger)
            : base(repositoryFacade, logger)
        {
        }
    }
}
