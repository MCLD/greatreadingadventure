using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace GRA.Domain.Service.Abstract
{
    public abstract class BaseService<T>
    {
        protected readonly ILogger<T> logger;
        public BaseService(ILogger<T> logger)
        {
            this.logger = Require.IsNotNull(logger, nameof(logger));
        }

        protected int GetUserId(ClaimsPrincipal user)
        {
            var userId = user.Claims
                .Where(_ => _.Type == ClaimType.UserId).SingleOrDefault().Value;
            return int.Parse(userId);
        }
    }
}
