using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using System.Security.Claims;
using System.Collections.Generic;

namespace GRA.Controllers.Base
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot config;
        public Controller(ServiceFacade.Controller context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.config = context.config;

            // sensible default
            PageTitle = "Great Reading Adventure";
        }

        protected string PageTitle {
            set {
                ViewData[ViewDataKey.Title] = value;
            }
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

        protected async void LoginUser(AuthenticationResult authResult)
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

                await HttpContext.Authentication.SignInAsync(Authentication.SchemeGRACookie,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, Authentication.TypeGRAPassword)));

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

        protected async void LogoutUser()
        {
            HttpContext.Session.Remove(SessionKey.User);
            await HttpContext.Authentication.SignOutAsync(Authentication.SchemeGRACookie);
        }

        protected ClaimsIdentity CurrentUser {
            get {
                return (ClaimsIdentity)HttpContext.User.Identity;
            }
        }

        protected bool UserHasPermission(Permission permission)
        {
            return HttpContext.User.HasClaim(ClaimType.Permission, permission.ToString());
        }

        protected string UserClaim(string claimType)
        {
            var claim = HttpContext
                .User
                .Claims
                .Where(_ => _.Type == claimType)
                .SingleOrDefault();

            if(claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
