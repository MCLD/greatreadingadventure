using GRA.Domain.Service.Abstract;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace GRA.Controllers
{
    public class UserContextProvider : IUserContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteLookupService _simpleSiteService;
        public UserContextProvider(IHttpContextAccessor httpContextAccessor,
            SiteLookupService simpleSiteService)
        {
            _httpContextAccessor = Require.IsNotNull(httpContextAccessor,
                nameof(httpContextAccessor));
            _simpleSiteService = Require.IsNotNull(simpleSiteService, nameof(simpleSiteService));
        }
        public UserContext GetContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userContext = new UserContext
            {
                User = httpContext.User,
                SiteId = (int)httpContext.Items[SessionKey.SiteId]
            };

            if (httpContext.User.Identity.IsAuthenticated)
            {
                userContext.ActiveUserId = httpContext.Session.GetInt32(SessionKey.ActiveUserId)
                    ?? new UserClaimLookup(httpContext.User).GetId(ClaimType.UserId);
            }
            return userContext;
        }
    }
}
