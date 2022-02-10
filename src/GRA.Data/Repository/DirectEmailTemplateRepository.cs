using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DirectEmailTemplateRepository
        : AuditingRepository<Model.DirectEmailTemplate, Domain.Model.DirectEmailTemplate>,
        IDirectEmailTemplateRepository
    {
        internal DirectEmailTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<DirectEmailTemplateRepository> logger)
            : base(repositoryFacade, logger)
        {
        }
    }
}
