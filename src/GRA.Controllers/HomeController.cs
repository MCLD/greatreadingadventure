using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSite(sitePath);
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if(CurrentUser.Identity.IsAuthenticated)
            {
                return View("Dashboard");
            }
            else
            {
                return View();
            }
        }
    }
}
