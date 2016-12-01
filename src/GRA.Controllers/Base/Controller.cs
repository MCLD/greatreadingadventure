using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GRA.Domain.Service;
using GRA.Controllers.Helpers;

namespace GRA.Controllers.Base
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot _config;
        protected readonly SiteService _siteService;
        protected string PageTitle { get; set; }
        public Controller(ServiceFacade.Controller context)
        {
            _config = context.Config;
            _siteService = context.SiteService;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // sensible default
            string pageTitle = _config[ConfigurationKeys.DefaultSiteName];

            // set page title
            var controller = context.Controller as Controller;
            if (controller != null && !string.IsNullOrWhiteSpace(controller.PageTitle))
            {
                pageTitle = controller.PageTitle;
            }
            ViewData[ViewDataKey.Title] = pageTitle;
        }

        protected string AlertDanger {
            set {
                TempData[TempDataKey.AlertDanger] = value;
            }
        }

        protected string AlertWarning {
            set {
                TempData[TempDataKey.AlertWarning] = value;
            }
        }
        protected string AlertInfo {
            set {
                TempData[TempDataKey.AlertInfo] = value;
            }
        }
        protected string AlertSuccess {
            set {
                TempData[TempDataKey.AlertSuccess] = value;
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        protected async Task LoginUserAsync(AuthenticationResult authResult)
        {
            if (authResult.FoundUser && authResult.PasswordIsValid)
            {
                var claims = new HashSet<Claim>();
                foreach (var permissionName in authResult.PermissionNames)
                {
                    claims.Add(new Claim(ClaimType.Permission, permissionName));
                }
                claims.Add(new Claim(ClaimType.BranchId, authResult.User.BranchId.ToString()));
                claims.Add(new Claim(ClaimType.SiteId, authResult.User.SiteId.ToString()));
                claims.Add(new Claim(ClaimType.SystemId, authResult.User.SystemId.ToString()));
                claims.Add(new Claim(ClaimType.UserId, authResult.User.Id.ToString()));

                var identity = new ClaimsIdentity(claims, Authentication.TypeGRAPassword);

                await HttpContext.Authentication.SignInAsync(Authentication.SchemeGRACookie,
                    new ClaimsPrincipal(identity));

                if (!string.IsNullOrEmpty(authResult.AuthenticationMessage))
                {
                    AlertInfo = authResult.AuthenticationMessage;
                }
            }
            else
            {
                AlertDanger = authResult.AuthenticationMessage;
            }
        }

        protected async Task LogoutUserAsync()
        {
            await HttpContext.Authentication.SignOutAsync(Authentication.SchemeGRACookie);
        }

        protected ClaimsPrincipal CurrentUser {
            get {
                return HttpContext.User;
            }
        }

        protected bool UserHasPermission(Permission permission)
        {
            return new UserClaimLookup(CurrentUser).UserHasPermission(permission.ToString());
        }

        protected string UserClaim(string claimType)
        {
            return new UserClaimLookup(CurrentUser).UserClaim(claimType);
        }

        protected int GetId(string claimType)
        {
            return new UserClaimLookup(CurrentUser).GetId(claimType);
        }

        protected async Task<int> GetCurrentSiteId(string sitePath)
        {
            return await new SiteHelper(_siteService).GetSiteId(HttpContext, sitePath);

        }

        protected async Task<Site> GetCurrentSite(string sitePath)
        {
            return await _siteService.GetById(await GetCurrentSiteId(sitePath));
        }
    }
}
