using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using GRA.Controllers;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Base
{
    public abstract class BaseCommand : CommandLineApplication
    {
        protected readonly ServiceFacade _facade;
        private User _user;
        private Site _site;

        protected User User
        {
            get
            {
                if(_user == null)
                {
                    var task = Task.Run(() => EnsureUserAndSiteLoaded());
                    task.Wait();
                }
                return _user;
            }
            private set
            {
                _user = value;
            }
        }
        
        protected Site Site
        {
            get
            {
                if(_site == null)
                {
                    var task = Task.Run(() => EnsureUserAndSiteLoaded());
                    task.Wait();
                }
                return _site;
            }
            private set
            {
                _site = value;
            }
        }

        public BaseCommand(ServiceFacade serviceFacade)
        {
            _facade = serviceFacade ?? throw new ArgumentNullException(nameof(serviceFacade));
        }

        protected async Task EnsureUserAndSiteLoaded()
        {
            if(_site != null && _user != null)
            {
                return;
            }

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

            User = await _facade.UserService.GetDetails(userId);

            // look up the user's site
            Site = await _facade.SiteLookupService.GetByIdAsync(User.SiteId);

            // update the context SiteId in case the user's is set differently than the config file
            ctxt.Items[ItemKey.SiteId] = User.SiteId;
            // compute the proper site stage
            ctxt.Items[ItemKey.SiteStage] = _facade.SiteLookupService.GetSiteStageAsync(Site);

            // now that we've got the user's information let's build the real principal
            ctxt.User = BuildClaimsPrincipal(User);

            // clear the cached user context provider
            _facade.UserService.ClearCachedUserContext();

            Console.WriteLine($"Running as user: {User.Username} on site: {Site.Name}");
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

        private ClaimsPrincipal BuildClaimsPrincipal(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var claims = new HashSet<Claim>();
            foreach (var permission in Enum.GetValues(typeof(Domain.Model.Permission)))
            {
                claims.Add(new Claim(ClaimType.Permission, permission.ToString()));
            }
            claims.Add(new Claim(ClaimType.BranchId, user.BranchId.ToString()));
            claims.Add(new Claim(ClaimType.SiteId, user.SiteId.ToString()));
            claims.Add(new Claim(ClaimType.SystemId, user.SystemId.ToString()));
            claims.Add(new Claim(ClaimType.UserId, user.Id.ToString()));

            var identity = new ClaimsIdentity(claims, Authentication.TypeGRAPassword);

            return new GenericPrincipal(identity, null);
        }
    }
}
