using System;

using GRA.Domain.Service;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GRA.Controllers.Base
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        protected readonly IConfigurationRoot config;
        protected readonly UserManager<Domain.Model.User> userManager;
        public Controller(ServiceFacade.Controller context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.config = context.config;
            this.userManager = context.userManager;

            // sensible default
            PageTitle = "Great Reading Adventure";
        }

        protected string PageTitle {
            set {
                ViewData[GRA.ViewDataName.Title] = value;
            }
        }

        protected string AlertDanger {
            set {
                TempData[GRA.TempDataName.AlertDanger] = value;
            }
        }

        protected string AlertWarning {
            set {
                TempData[GRA.TempDataName.AlertWarning] = value;
            }
        }
        protected string AlertInfo {
            set {
                TempData[GRA.TempDataName.AlertInfo] = value;
            }
        }
        protected string AlertSuccess {
            set {
                TempData[GRA.TempDataName.AlertSuccess] = value;
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
