using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Service.Abstract;

namespace GRA.Controllers.Filter
{
    public class SiteFilter : Attribute, IAsyncResourceFilter
    {
        private readonly ILogger<SiteFilter> _logger;
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserContextProvider _userContextProvider;
        public SiteFilter(ILogger<SiteFilter> logger,
            SiteLookupService siteLookupService,
            IUserContextProvider userContextProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userContextProvider = userContextProvider 
                ?? throw new ArgumentNullException(nameof(userContextProvider));
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,
            ResourceExecutionDelegate next)
        {
            Site site = null;
            var httpContext = context.HttpContext;
            // if we've already fetched it on this request it's present in Items
            int? siteId = null;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // if the user is authenticated, that is their site
                try
                {
                    siteId = _userContextProvider.GetId(httpContext.User, ClaimType.SiteId);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to get SiteId claim for user {httpContext.User.Identity.Name}: {ex.Message}");
                }
            }

            if (siteId == null)
            {
                string sitePath = context.RouteData.Values["sitePath"]?.ToString();
                // first check, did they use a sitePath giving them a specific site
                if (!string.IsNullOrEmpty(sitePath))
                {
                    site = await _siteLookupService.GetSiteByPathAsync(sitePath);
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
                    siteId = await _siteLookupService.GetDefaultSiteIdAsync();
                }
            }
            if (site == null)
            {
                site = await _siteLookupService.GetByIdAsync((int)siteId);
            }

            httpContext.Items[ItemKey.GoogleAnalytics] = site.GoogleAnalyticsTrackingId;
            httpContext.Items[ItemKey.SiteStage] = _siteLookupService.GetSiteStageAsync(site);
            httpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            httpContext.Items[ItemKey.SiteId] = (int)siteId;

            await next();
        }
    }
}
