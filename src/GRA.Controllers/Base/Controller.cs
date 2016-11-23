using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using GRA.Controllers.Extension;

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

        protected void LoginUser(AuthenticatedUser user)
        {
            if (user.Authenticated)
            {
                currentUser = user;
                HttpContext.Session.SetObjectAsJson(SessionKey.User, user);
                if (!string.IsNullOrEmpty(user.AuthenticationMessage))
                {
                    AlertInfo = user.AuthenticationMessage;
                }
            }
            else
            {
                AlertDanger = user.AuthenticationMessage;
            }
        }

        protected void LogoutUser()
        {
            HttpContext.Session.Remove(SessionKey.User);
            currentUser = null;
        }

        private AuthenticatedUser currentUser { get; set; }
        protected AuthenticatedUser CurrentUser {
            get {
                if(currentUser == null)
                {
                    currentUser = HttpContext.Session.GetObjectFromJson<AuthenticatedUser>(SessionKey.User);
                }
                return currentUser;
            }
        }
    }
}
