using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            HttpContext.Items["sitePath"] = sitePath;
            var site = await GetCurrentSite(sitePath);
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if (CurrentUser.Identity.IsAuthenticated)
            {
                return View("Dashboard");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Signout()
        {
            if (CurrentUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { action = "Index" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogBook(GRA.Domain.Model.Book book)
        {
            await _activityService.LogActivityAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId), 1);
            if (!string.IsNullOrWhiteSpace(book.Title))
            {
                await _activityService.AddBook((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId), book);
            }
            return RedirectToAction("Index");
        }
    }
}
