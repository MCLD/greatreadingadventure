using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> logger;
        public HomeController(ILogger<HomeController> logger, ServiceFacade.Controller context)
            : base(context)
        {
            this.logger = logger;
        }

        public IActionResult Index(string site = null)
        {
            return View();
        }
    }
}
