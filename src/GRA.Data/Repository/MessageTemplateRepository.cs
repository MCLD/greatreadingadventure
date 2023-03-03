using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class MessageTemplateRepository
        : AuditingRepository<Model.MessageTemplate, Domain.Model.MessageTemplate>,
        IMessageTemplateRepository
    {
        public MessageTemplateRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<MessageTemplateRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
