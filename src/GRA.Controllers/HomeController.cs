using GRA.Controllers.ViewModel;
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
        private const string AuthorMissingTitle = "AuthorMissingTitle";

        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSite(sitePath);
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if (AuthUser.Identity.IsAuthenticated)
            {
                var user = await _userService.GetDetails(GetActiveUserId());
                DashboardViewModel viewModel = new DashboardViewModel()
                {
                    FirstName = user.FirstName,
                    CurrentPointTotal = user.PointsEarned
                };
                if (TempData.ContainsKey(AuthorMissingTitle))
                {
                    viewModel.Author = (string)TempData[AuthorMissingTitle];
                    ModelState.AddModelError("Title", "Please include the Title of the book");
                }
                
                return View("Dashboard", viewModel);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { action = "Index" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogBook(DashboardViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Title)
                && !string.IsNullOrWhiteSpace(viewModel.Author))
            {
                TempData[AuthorMissingTitle] = viewModel.Author;
            }
            else
            {
                var book = new Domain.Model.Book
                {
                    Author = viewModel.Author,
                    Title = viewModel.Title
                };
                var result = await _activityService.LogActivityAsync(GetActiveUserId(), 1);
                string message = $"<span class=\"fa fa-star\"></span> You earned <strong>{result.PointsEarned} points</strong> and currently have <strong>{result.User.PointsEarned} points</strong>!";
                if (!string.IsNullOrWhiteSpace(book.Title))
                {
                    await _activityService.AddBook(GetActiveUserId(), book);
                    message += $" The book <strong><em>{book.Title}</em> by {book.Author}</strong> was added to your book list.";
                }
                AlertSuccess = message;
            }
            return RedirectToAction("Index");
        }
    }
}
