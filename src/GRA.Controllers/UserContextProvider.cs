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
        public async Task<UserContext> GetContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return new UserContext
            {
                User = httpContext.User,
                SiteId = await GetSiteId()
            };
        }

        private async Task<int> GetSiteId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string sitePath = httpContext.Items["sitePath"] as string;

            int? siteId = (int?)httpContext.Items[SessionKey.SiteId];
            if (siteId == null)
            {
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    // if the user is authenticated, that is their site
                    siteId = new UserClaimLookup(httpContext.User).GetId(ClaimType.SiteId);
                }
                else
                {
                    // first check, did they use a sitePath giving them a specific site
                    if (!string.IsNullOrEmpty(sitePath))
                    {
                        var site = await _simpleSiteService.GetSiteByPath(sitePath);
                        if (site != null)
                        {
                            siteId = site.Id;
                        }
                    }
                    // if not check if they already have one in their session
                    if (siteId == null)
                    {
                        siteId = httpContext.Session.GetInt32(SessionKey.SiteId);
                    }
                    // if not then resort to the default
                    if (siteId == null)
                    {
                        siteId = await _simpleSiteService.GetDefaultSiteId();
                    }
                }
            }
            httpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            httpContext.Items[SessionKey.SiteId] = (int)siteId;
            return (int)siteId;
        }
    }
}
