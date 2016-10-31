using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

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

        public IActionResult Index(string site = null)
        {
            var siteList = service.GetSitePaths();
            if (siteList.Count() == 0)
            {
                logger.LogInformation("Site list from database is empty");
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
