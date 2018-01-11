using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using GRA.Controllers;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace GRA.CommandLine.FakeWeb
{
    public class ConfigureUserSite
    {
        private readonly ServiceFacade _facade;
        public ConfigureUserSite(ServiceFacade serviceFacade)
        {
            _facade = serviceFacade ?? throw new ArgumentNullException(nameof(serviceFacade));
        }
        public async Task<(User User, Site Site)> Lookup(int? emulateUserId = null)
        {
            User user = null;
            Site site = null;
            int userId;
            if (!string.IsNullOrEmpty(_facade.Config["GRACL.UserId"]))
            {
                if (!int.TryParse(_facade.Config["GRACL.UserId"], out userId))
                {
                    throw new ArgumentException("Configuration value GRACL.UserId must be a number.");
                }
            }
            else
            {
                throw new Exception("Configuration value GRACL.UserId must be set.");
            }

            int siteId;
            if (!string.IsNullOrEmpty(_facade.Config["GRACL.SiteId"]))
            {
                if (!int.TryParse(_facade.Config["GRACL.SiteId"], out siteId))
                {
                    throw new ArgumentException("Configuration value GRACL.SiteId must be a number.");
                }
            }
            else
            {
                throw new Exception("Configuration value GRACL.SiteId must be set.");
            }

            var ctxt = _facade.HttpContextAccessor.HttpContext;

            // initial fetch based on supplied user id and site id
            // don't use this principal for anything else as it doesn't have systemid and branchid
            ctxt.Items[ItemKey.SiteId] = siteId;
            ctxt.Items[ItemKey.SiteStage] = SiteStage.Unknown;
            ctxt.Session.SetInt32(SessionKey.ActiveUserId, userId);
            ctxt.User = BuildBrokenClaimsPrincipal(userId: userId, siteId: siteId);

            user = await _facade.UserService.GetDetails(emulateUserId ?? userId);
            // now that we've got the user's information let's build the real principal
            ctxt.Session.SetInt32(SessionKey.ActiveUserId, user.Id);
            var authentication = await _facade.AuthenticationService.RevalidateUserAsync(user.Id);
            ctxt.User = BuildClaimsPrincipal(authentication);

            // look up the user's site
            site = await _facade.SiteLookupService.GetByIdAsync(user.SiteId);

            // update the context SiteId in case the user's is set differently than the config file
            ctxt.Items[ItemKey.SiteId] = user.SiteId;
            // compute the proper site stage
            ctxt.Items[ItemKey.SiteStage] = _facade.SiteLookupService.GetSiteStage(site);

            // clear the cached user context provider
            _facade.UserService.ClearCachedUserContext();

            if (emulateUserId == null)
            {
                Console.WriteLine($"Running as user id: {user.Id} ({user.Username}) on site: {site.Name}");
            }

            return (user, site);
        }

        private ClaimsPrincipal BuildBrokenClaimsPrincipal(int userId, int siteId)
        {
            var claims = new HashSet<Claim>();
            foreach (var permission in Enum.GetValues(typeof(Domain.Model.Permission)))
            {
                claims.Add(new Claim(ClaimType.Permission, permission.ToString()));
            }
            // If this were a real ClaimsPrincipal it would need to have ClaimTypes BranchId and
            // SystemId. This ClaimsPrincipal is just to get the details of the user id specified
            // in configuration.
            claims.Add(new Claim(ClaimType.SiteId, siteId.ToString()));
            claims.Add(new Claim(ClaimType.UserId, userId.ToString()));

            var identity = new ClaimsIdentity(claims, Authentication.TypeGRAPassword);

            return new GenericPrincipal(identity, null);
        }

        private ClaimsPrincipal BuildClaimsPrincipal(AuthenticationResult authenticationResult)
        {
            if (authenticationResult == null)
            {
                throw new ArgumentNullException(nameof(authenticationResult));
            }
            var claims = new HashSet<Claim>();
            foreach(var permission in authenticationResult.PermissionNames)
            {
                claims.Add(new Claim(ClaimType.Permission, permission));
            }
            claims.Add(new Claim(ClaimType.BranchId, authenticationResult.User.BranchId.ToString()));
            claims.Add(new Claim(ClaimType.SiteId, authenticationResult.User.SiteId.ToString()));
            claims.Add(new Claim(ClaimType.SystemId, authenticationResult.User.SystemId.ToString()));
            claims.Add(new Claim(ClaimType.UserId, authenticationResult.User.Id.ToString()));

            var identity = new ClaimsIdentity(claims, Authentication.TypeGRAPassword);

            return new GenericPrincipal(identity, null);
        }

    }
}
