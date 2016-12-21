using GRA.Controllers.ViewModel;
using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Base.UserController
    {
        private const string AuthorMissingTitle = "AuthorMissingTitle";
        private const int BadgesToDisplay = 6;

        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        private readonly EmailReminderService _emailReminderService;
        private readonly SiteService _siteService;
        private readonly StaticAvatarService _staticAvatarService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            EmailReminderService emailReminderService,
            SiteService siteService,
            StaticAvatarService staticAvatarService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _emailReminderService = Require.IsNotNull(emailReminderService,
                nameof(emailReminderService));
            _staticAvatarService = Require.IsNotNull(staticAvatarService,
                nameof(staticAvatarService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSiteAsync();
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if (AuthUser.Identity.IsAuthenticated)
            {
                // signed-in users can view the dashboard
                var user = await _userService.GetDetails(GetActiveUserId());
                StaticAvatar avatar = new StaticAvatar();
                if (user.AvatarId != null)
                {
                    avatar = await _staticAvatarService.GetByIdAsync(user.AvatarId.Value);
                    avatar.Filename = ResolveContentPath(avatar.Filename);
                }

                var badges = await _userService.GetPaginatedBadges(user.Id, 0, BadgesToDisplay);
                foreach (var badge in badges.Data)
                {
                    badge.Filename = ResolveContentPath(badge.Filename);
                }
                DashboardViewModel viewModel = new DashboardViewModel()
                {
                    FirstName = user.FirstName,
                    CurrentPointTotal = user.PointsEarned,
                    AvatarPath = avatar.Filename,
                    Badges = badges.Data
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
                // TODO handle pages if they are assigned in lieu of views
                switch (GetSiteStage())
                {
                    case SiteStage.BeforeRegistration:
                        var viewModel = new BeforeRegistrationViewModel
                        {
                            Site = await GetCurrentSiteAsync(),
                            SignUpSource = "BeforeRegistration"
                        };
                        if (viewModel.Site != null && viewModel.Site.RegistrationOpens != null)
                        {
                            viewModel.RegistrationOpens
                                = ((DateTime)viewModel.Site.RegistrationOpens).ToString("D");
                        }
                        return View("IndexBeforeRegistration", viewModel);
                    case SiteStage.RegistrationOpen:
                        return View("IndexRegistrationOpen");
                    case SiteStage.ProgramEnded:
                        return View("IndexProgramEnded");
                    case SiteStage.AccessClosed:
                        return View("IndexAccessClosed");
                    case SiteStage.ProgramOpen:
                    default:
                        return View("IndexProgramOpen");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReminder(BeforeRegistrationViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Email))
            {
                await _emailReminderService
                    .AddEmailReminderAsync(viewModel.Email, viewModel.SignUpSource);
            }
            AlertInfo = "<span class=\"fa fa-envelope\"></span> Thanks! We'll let you know when you can join the program.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetIcs()
        {
            var site = await GetCurrentSiteAsync();
            var filename = new string(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var calendarBytes = await _siteService.GetIcsFile(siteUrl);
            return File(calendarBytes, "text/calendar", $"{filename}.ics");
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToAction("Index");
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
                await _activityService.LogActivityAsync(GetActiveUserId(), 1, book);
            }
            return RedirectToAction("Index");
        }
    }
}