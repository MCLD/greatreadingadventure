using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
            this.logger = Require.IsNotNull(logger, nameof(logger));
            PageTitle = "Mission Control";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            if (!CurrentUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new { controller = "Login" });
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to main site
                return RedirectToRoute(new { area = string.Empty });
            }
            Site site = await GetCurrentSite(sitePath);
            PageTitle = $"Mission Control: {site.Name}";
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            if (CurrentUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { area = string.Empty, action = "Index" });
        }
    }
}
