using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class EmailTemplateRepository
        : AuditingRepository<Model.EmailTemplate, Domain.Model.EmailTemplate>,
        IEmailTemplateRepository
    {
        public EmailTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<EmailTemplateRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
