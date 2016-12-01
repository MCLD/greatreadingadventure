using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.Helpers
{
    public class SiteHelper
    {
        private readonly SiteService _siteService;
        public SiteHelper(SiteService siteService)
        {
            _siteService = siteService;
        }
        public async Task<int> GetSiteId(HttpContext context, string sitePath)
        {
            int? siteId = (int?)context.Items[SessionKey.SiteId];
            if (siteId == null)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    // if the user is authenticated, that is their site
                    siteId = new UserClaimLookup(context.User).GetId(ClaimType.SiteId);
                }
                else
                {
                    // first check, did they use a sitePath giving them a specific site
                    if (!string.IsNullOrEmpty(sitePath))
                    {
                        var site = await _siteService.GetSiteByPath(sitePath);
                        if (site != null)
                        {
                            siteId = site.Id;
                        }
                    }
                    // if not check if they already have one in their session
                    if (siteId == null)
                    {
                        siteId = context.Session.GetInt32(SessionKey.SiteId);
                    }
                    // if not then resort to the default
                    if (siteId == null)
                    {
                        siteId = await _siteService.GetDefaultSiteId();
                    }
                }
            }
            context.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            context.Items[SessionKey.SiteId] = (int)siteId;
            return (int)siteId;
        }
    }
}
