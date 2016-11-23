using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
            if(logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
        }

        //[Authorize(Policy = PolicyName.MissionControlAccess)]
        public IActionResult Index()
        {
            if (CurrentUser == null 
                || CurrentUser.Permissions == null 
                || CurrentUser.Permissions.Count() == 0)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new { controller = "Login" });
            }

            if (!CurrentUser.Permissions.Contains(Domain.Model.Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to main site
                return RedirectToRoute(new { area = string.Empty });
            }

            return View();
        }
    }
}
