using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            this.logger = logger;
        }

        //[Authorize(Policy = PolicyName.MissionControlAccess)]
        public IActionResult Index()
        {
            if (User.Claims.Count() == 0)
            {
                // not logged in
                return RedirectToRoute(new { controller = "Login" });
            }

            if (User.Claims
                .Where(c => c.Type == ClaimType.Privilege && c.Value == ClaimName.MissionControlUser)
                .FirstOrDefault() == null)
            {
                // not authorized for Mission Control
                return RedirectToRoute(new { area = string.Empty });
            }

            return View();
        }
    }
}
