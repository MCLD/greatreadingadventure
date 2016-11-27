using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace GRA.Controllers.Base
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot config;
        protected string PageTitle { get; set; }
        public Controller(ServiceFacade.Controller context)
        {
            config = context.config;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // sensible default
            string pageTitle = "Great Reading Adventure";

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
            HttpContext.Session.Remove(SessionKey.User);
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
    }
}
