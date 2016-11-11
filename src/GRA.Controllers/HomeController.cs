using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        public HomeController(ILogger<HomeController> logger, ServiceFacade.Controller context) 
            : base(context)
        {
            this.logger = logger;
        }

        public async Task<IActionResult> Index(string site = null)
        {
            var siteList = service.GetSitePaths();
            if (siteList.Count() == 0)
            {
                logger.LogInformation("Site list from database is empty");
                var user = new Domain.Model.Participant
                {
                    UserName = "admin"
                };
                IdentityResult result = await userManager.CreateAsync(user, "changeMe06!");
                if (result.Succeeded)
                {
                    logger.LogInformation("Created admin account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        logger.LogError($"Problem creating admin account: {error.Description}");
                    }
                }
            }
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
