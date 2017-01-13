using GRA.Domain.Service.Abstract;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using GRA.Domain.Model;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class UserContextProvider : IUserContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SiteLookupService _siteLookupService;

        public UserContextProvider(IHttpContextAccessor httpContextAccessor,
            SiteLookupService siteLookupService)
        {
            _httpContextAccessor = Require.IsNotNull(httpContextAccessor,
                nameof(httpContextAccessor));
            _siteLookupService = Require.IsNotNull(siteLookupService, nameof(siteLookupService));
        }
        public UserContext GetContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userContext = new UserContext
            {
                User = httpContext.User,
                SiteId = (int)httpContext.Items[ItemKey.SiteId],
                SiteStage = (SiteStage)httpContext.Items[ItemKey.SiteStage]
            };

            if (httpContext.User.Identity.IsAuthenticated)
            {
                userContext.ActiveUserId = httpContext.Session.GetInt32(SessionKey.ActiveUserId)
                    ?? new UserClaimLookup(httpContext.User).GetId(ClaimType.UserId);
            }
            return userContext;
        }

        public async Task<Site> GetCurrentSiteAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return await _siteLookupService.GetByIdAsync((int)httpContext.Items[ItemKey.SiteId]);
        }
    }
}
