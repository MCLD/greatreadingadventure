using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Base
{
    public abstract class BaseCommand : CommandLineApplication
    {
        protected ServiceFacade _facade;
        protected User _user;
        protected Site _site;

        public BaseCommand(ServiceFacade serviceFacade)
        {
            _facade = serviceFacade ?? throw new ArgumentNullException(nameof(serviceFacade));
        }

        protected async Task LoadUserAndSiteAsync()
        {
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

            //var userLookup = Task.Run(() => _facade.UserService.GetDetails(userId));
            //userLookup.Wait();
            //_user = userLookup.Result;
            _user = await _facade.UserService.GetDetails(userId);

            // now that we've got the user's information let's build the real principal
            ctxt.User = BuildClaimsPrincipal(_user);

            // update the context SiteId in case the user's is set differently than the config file
            ctxt.Items[ItemKey.SiteId] = _user.SiteId;

            //var siteLookup = Task.Run(() => _facade.SiteLookupService.GetByIdAsync(_user.SiteId));
            //siteLookup.Wait();
            //_site = siteLookup.Result;
            _site = await _facade.SiteLookupService.GetByIdAsync(_user.SiteId);

            Console.WriteLine($"Running as user: {_user.Username} on site: {_site.Name}");
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
