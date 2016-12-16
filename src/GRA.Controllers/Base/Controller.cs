using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using GRA.Controllers.Filter;
using System.Security.Principal;

namespace GRA.Controllers.Base
{
    [ServiceFilter(typeof(SiteFilter))]
    [SessionTimeoutFilter]
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot _config;
        protected readonly IUserContextProvider _userContextProvider;
        protected readonly SiteLookupService _siteLookupService;
        protected string PageTitle { get; set; }
        public Controller(ServiceFacade.Controller context)
        {
            _config = context.Config;
            _userContextProvider = context.UserContextProvider;
            _siteLookupService = context.SiteLookupService;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // sensible default
            string pageTitle = _config[ConfigurationKey.DefaultSiteName];

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

                HttpContext.Session.SetInt32(SessionKey.ActiveUserId, authResult.User.Id);
                HttpContext.Items[SessionKey.ActiveUserId] = authResult.User.Id;

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

        public async Task LogoutUserAsync()
        {
            int? siteId = HttpContext.Session.GetInt32(SessionKey.SiteId);
            HttpContext.Session.Clear();
            await HttpContext.Authentication.SignOutAsync(Authentication.SchemeGRACookie);
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            if (siteId != null)
            {
                HttpContext.Session.SetInt32(SessionKey.SiteId, (int)siteId);
            }
        }

        protected ClaimsPrincipal AuthUser {
            get {
                return HttpContext.User;
            }
        }

        protected bool UserHasPermission(Permission permission)
        {
            return new UserClaimLookup(AuthUser).UserHasPermission(permission.ToString());
        }

        protected string UserClaim(string claimType)
        {
            return new UserClaimLookup(AuthUser).UserClaim(claimType);
        }

        protected int GetId(string claimType)
        {
            return new UserClaimLookup(AuthUser).GetId(claimType);
        }

        protected int GetCurrentSiteId(string sitePath = null)
        {
            var context = _userContextProvider.GetContext();
            return context.SiteId;
        }

        protected async Task<Site> GetCurrentSiteAsync(string sitePath = null)
        {
            return await _siteLookupService.GetByIdAsync(GetCurrentSiteId(sitePath));
        }

        protected int GetActiveUserId()
        {
            int? activeUserId = HttpContext.Session.GetInt32(SessionKey.ActiveUserId);
            if(activeUserId == null)
            {
                activeUserId = GetId(ClaimType.UserId);
            }
            return (int)activeUserId;
        }

        protected SiteStage GetSiteStage()
        {
            return (SiteStage)HttpContext.Items[ItemKey.SiteStage];
        }

        protected string ResolveContentPath(string filePath)
        {
            string path = _config[ConfigurationKey.ContentPath];
            if(!path.EndsWith("/"))
            {
                path += "/";
            }
            return path + filePath;
        }
    }
}
