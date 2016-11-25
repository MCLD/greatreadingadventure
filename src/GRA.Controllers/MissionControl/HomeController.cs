using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> logger;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context)
            : base(context)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
        }

        public IActionResult Index()
        {
            if (!CurrentUser.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new { controller = "Login" });
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to main site
                return RedirectToRoute(new { area = string.Empty });
            }

            return View();
        }
    }
}
