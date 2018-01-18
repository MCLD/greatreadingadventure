using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using System.IO;

namespace GRA.Controllers
{
    public class HomeController : Base.UserController
    {
        private const string ActivityErrorMessage = "ActivityErrorMessage";
        private const string TitleErrorMessage = "TitleErrorMessage";
        private const string AuthorErrorMessage = "AuthorErrorMessage";
        private const string ModelData = "ModelData";
        private const string SecretCodeMessage = "SecretCodeMessage";
        private const int BadgesToDisplay = 6;

        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly DashboardContentService _dashboardContentService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly EmailReminderService _emailReminderService;
        private readonly PageService _pageService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            DailyLiteracyTipService dailyLiteracyTipService,
            DashboardContentService dashboardContentService,
            DynamicAvatarService dynamicAvatarService,
            EmailReminderService emailReminderService,
            PageService pageService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _dailyLiteracyTipService = Require.IsNotNull(dailyLiteracyTipService,
                nameof(dailyLiteracyTipService));
            _dashboardContentService = Require.IsNotNull(dashboardContentService,
                nameof(dashboardContentService));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _emailReminderService = Require.IsNotNull(emailReminderService,
                nameof(emailReminderService));
            _pageService = Require.IsNotNull(pageService, nameof(pageService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task<IActionResult> Index()
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

                var badges = await _userService.GetPaginatedBadges(user.Id, 0, BadgesToDisplay);
                foreach (var badge in badges.Data)
                {
                    badge.Filename = _pathResolver.ResolveContentPath(badge.Filename);
                }

                var pointTranslation = await _activityService.GetUserPointTranslationAsync();
                DashboardViewModel viewModel = new DashboardViewModel()
                {
                    FirstName = user.FirstName,
                    CurrentPointTotal = user.PointsEarned,
                    SingleEvent = pointTranslation.IsSingleEvent,
                    ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural,
                    Badges = badges.Data,
                    DisableSecretCode = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable)
                };

                var program = await _siteService.GetProgramByIdAsync(user.ProgramId);
                if (program.DailyLiteracyTipId.HasValue)
                {
                    var day = _siteLookupService.GetSiteDay(site);
                    if (day.HasValue)
                    {
                        var image = await _dailyLiteracyTipService.GetImageByDayAsync(
                            program.DailyLiteracyTipId.Value, day.Value);
                        if (image != null)
                        {
                            var imagePath = _pathResolver.ResolveContentFilePath(
                                Path.Combine($"site{site.Id}", "dailyimages",
                                $"dailyliteracytip{program.DailyLiteracyTipId}",
                                $"{image.Id}.{image.Extension}"));
                            if (System.IO.File.Exists(imagePath))
                            {
                                viewModel.DailyImageMessage = program.DailyLiteracyTip.Message;
                                viewModel.DailyImagePath = imagePath;
                            }
                        }
                    }
                }

                var userDynamicAvatar = await _dynamicAvatarService.GetUserAvatarAsync();
                if (userDynamicAvatar?.Count > 0)
                {
                    var dynamicAvatarElements = userDynamicAvatar;
                    foreach (var element in dynamicAvatarElements)
                    {
                        element.Filename = _pathResolver.ResolveContentPath(element.Filename);
                    }
                    viewModel.DynamicAvatarElements = dynamicAvatarElements;
                }

                var dashboardPage = await _dashboardContentService.GetCurrentContentAsync();
                if (dashboardPage != null && !string.IsNullOrWhiteSpace(dashboardPage.Content))
                {
                    viewModel.DashboardPageContent = CommonMark.CommonMarkConverter
                        .Convert(dashboardPage.Content);
                }
                
                if (TempData.ContainsKey(ModelData))
                {
                    var model = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<DashboardViewModel>((string)TempData[ModelData]);
                    viewModel.ActivityAmount = model.ActivityAmount;
                    viewModel.Title = model.Title;
                    viewModel.Author = model.Author;
                }
                if (TempData.ContainsKey(ActivityErrorMessage))
                {
                    ModelState.AddModelError("ActivityAmount", (string)TempData[ActivityErrorMessage]);
                    viewModel.ActivityAmount = null;
                }
                if (TempData.ContainsKey(TitleErrorMessage))
                {
                    ModelState.AddModelError("Title", (string)TempData[TitleErrorMessage]);
                }
                if (TempData.ContainsKey(AuthorErrorMessage))
                {
                    ModelState.AddModelError("Author", (string)TempData[AuthorErrorMessage]);
                }
                if (TempData.ContainsKey(SecretCodeMessage))
                {
                    viewModel.SecretCodeMessage = (string)TempData[SecretCodeMessage];
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
                ShowAlertInfo("Thanks! We'll let you know when you can join the program.", "envelope");
            }
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

        [PreventQuestionnaireRedirect]
        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult LogActivity()
        {
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogActivity(DashboardViewModel viewModel)
        {
            bool valid = true;
            if (!viewModel.SingleEvent
                && (viewModel.ActivityAmount == null || viewModel.ActivityAmount <= 0))
            {
                valid = false;
                TempData[ActivityErrorMessage] = "Please enter a whole number greater than 0.";
            }
            if (!ModelState.IsValid)
            {
                valid = false;
                if (ModelState["Title"].Errors.Count > 0)
                {
                    TempData[TitleErrorMessage] = ModelState["Title"].Errors.First().ErrorMessage;
                }
                if (ModelState["Author"].Errors.Count > 0)
                {
                    TempData[AuthorErrorMessage] = ModelState["Author"].Errors.First().ErrorMessage;
                }
            }
            if (string.IsNullOrWhiteSpace(viewModel.Title)
                && !string.IsNullOrWhiteSpace(viewModel.Author))
            {
                valid = false;
                TempData[TitleErrorMessage] = "Please include the Title of the book";
            }
            if (!valid)
            {
                TempData[ModelData] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel);
            }
            else
            {
                var book = new Domain.Model.Book
                {
                    Author = viewModel.Author,
                    Title = viewModel.Title
                };

                try
                {
                    await _activityService
                        .LogActivityAsync(GetActiveUserId(), viewModel.ActivityAmount ?? 1, book);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not log activity: ", gex);
                }
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogSecretCode(DashboardViewModel viewModel)
        {
            try
            {
                await _activityService.LogSecretCodeAsync(GetActiveUserId(), viewModel.SecretCode);
            }
            catch (GraException gex)
            {
                TempData[SecretCodeMessage] = gex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}