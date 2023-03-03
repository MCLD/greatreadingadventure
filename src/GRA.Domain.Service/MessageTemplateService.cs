using GRA.Abstract;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class MessageTemplateService : BaseUserService<MessageTemplateService>
    {
        public MessageTemplateService(ILogger<MessageTemplateService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider) : base(logger, dateTimeProvider, userContextProvider)
        {
        }
    }
}
