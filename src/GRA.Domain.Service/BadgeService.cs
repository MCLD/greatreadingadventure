using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class BadgeService : Abstract.BaseUserService<BadgeService>
    {
        public BadgeService(ILogger<BadgeService> logger, 
            IUserContextProvider userContextProvider) : base(logger, userContextProvider)
        {
        }
    }
}
