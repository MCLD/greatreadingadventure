using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        private readonly AvatarService _avatarService;
        private readonly CarouselService _carouselService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly DashboardContentService _dashboardContentService;
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailReminderService _emailReminderService;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public static string Name { get { return "Home"; } }

        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AvatarService avatarService,
            CarouselService carouselService,
            DailyLiteracyTipService dailyLiteracyTipService,
            DashboardContentService dashboardContentService,
            EmailManagementService emailManagementService,
            EmailReminderService emailReminderService,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _avatarService = avatarService
                ?? throw new ArgumentNullException(nameof(avatarService));
            _carouselService = carouselService
                ?? throw new ArgumentNullException(nameof(carouselService));
            _dailyLiteracyTipService = dailyLiteracyTipService
                ?? throw new ArgumentNullException(nameof(dailyLiteracyTipService));
            _dashboardContentService = dashboardContentService
                ?? throw new ArgumentNullException(nameof(dashboardContentService));
            _emailManagementService = emailManagementService
                ?? throw new ArgumentNullException(nameof(emailManagementService));
            _emailReminderService = emailReminderService
                ?? throw new ArgumentNullException(nameof(emailReminderService));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
        }

        public async Task<IActionResult> Index()
        {
            PageTitle = HttpContext.Items[ItemKey.SiteName].ToString();
            var site = await GetCurrentSiteAsync();
            if (AuthUser.Identity.IsAuthenticated)
            {
                if (TempData.ContainsKey(TempDataKey.UserJoined))
                {
                    if (UserHasPermission(Permission.AccessMissionControl))
                    {
                        if (GetSiteStage() == SiteStage.BeforeRegistration
                            || GetSiteStage() == SiteStage.AccessClosed)
                        {
                            return RedirectToAction(nameof(MissionControl.HomeController.Index),
                                HomeController.Name,
                                new { Area = nameof(MissionControl) });
                        }
                    }
                    else if (UserHasPermission(Permission.AccessPerformerRegistration))
                    {
                        var dates = await _performerSchedulingService.GetSettingsAsync();
                        var schedulingStage
                            = _performerSchedulingService.GetSchedulingStage(dates);

                        if (schedulingStage != PsSchedulingStage.Unavailable)
                        {
                            TempData.Remove(TempDataKey.UserJoined);
                            return RedirectToAction(
                                nameof(PerformerRegistration.HomeController.Index),
                                Name,
                                new { Area = nameof(PerformerRegistration) });
                        }
                    }
                }

                User user;
                // signed-in users can view the dashboard
                try
                {
                    user = await _userService.GetDetails(GetActiveUserId());
                }
                catch (GraException gex)
                {
                    // this likely means the currently authenticated user isn't valid anymore
                    _logger.LogError(gex,
                        "Problem displaying index for user {GetActiveUserId}: {Message}",
                        GetActiveUserId(),
                        gex.Message);
                    return RedirectToAction(nameof(SignOut));
                }

                var badges = await _userService.GetPaginatedBadges(user.Id, 0, BadgesToDisplay);
                foreach (var badge in badges.Data)
                {
                    badge.Filename = _pathResolver.ResolveContentPath(badge.Filename);
                }

                var pointTranslation = await _activityService.GetUserPointTranslationAsync();
                var viewModel = new DashboardViewModel
                {
                    FirstName = user.FirstName,
                    CurrentPointTotal = user.PointsEarned,
                    SingleEvent = pointTranslation.IsSingleEvent,
                    ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural,
                    Badges = badges.Data,
                    DisableSecretCode
                        = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable)
                };

                try
                {
                    viewModel.SiteStage = (SiteStage)HttpContext.Items[ItemKey.SiteStage];
                }
                catch (Exception)
                {
                    viewModel.SiteStage = SiteStage.Unknown;
                }

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
                            var imagePath = Path.Combine($"site{site.Id}",
                                "dailyimages",
                                $"dailyliteracytip{program.DailyLiteracyTipId}",
                                $"{image.Id}{image.Extension}");

                            if (System.IO.File.Exists(_pathResolver
                                .ResolveContentFilePath(imagePath)))
                            {
                                var dailyLiteracyTip = await _dailyLiteracyTipService
                                    .GetByIdAsync(program.DailyLiteracyTipId.Value);
                                viewModel.DailyImageLarge = dailyLiteracyTip.IsLarge;
                                viewModel.DailyImageMessage = dailyLiteracyTip.Message;
                                viewModel.DailyImagePath
                                    = _pathResolver.ResolveContentPath(imagePath);
                            }
                        }
                    }
                }

                if (TempData.ContainsKey(TempDataKey.UserJoined))
                {
                    TempData.Remove(TempDataKey.UserJoined);
                    viewModel.FirstTimeParticipant = user.IsFirstTime;
                    viewModel.ProgramName = program.Name;
                    viewModel.UserJoined = true;
                }

                var userAvatar = await _avatarService.GetUserAvatarAsync();
                if (userAvatar?.Count > 0)
                {
                    var avatarElements = userAvatar;
                    foreach (var element in avatarElements)
                    {
                        element.Filename = _pathResolver.ResolveContentPath(element.Filename);
                    }
                    viewModel.AvatarElements = avatarElements;
                }

                var dashboardPage = await _dashboardContentService.GetCurrentContentAsync();
                if (dashboardPage != null && !string.IsNullOrWhiteSpace(dashboardPage.Content))
                {
                    viewModel.DashboardPageContent = CommonMark.CommonMarkConverter
                        .Convert(dashboardPage.Content);
                }

                viewModel.Carousel = await _carouselService.GetCurrentForDashboardAsync();

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
                    ModelState.AddModelError("ActivityAmount",
                        _sharedLocalizer[(string)TempData[ActivityErrorMessage]]);
                    viewModel.ActivityAmount = null;
                }
                if (TempData.ContainsKey(TitleErrorMessage))
                {
                    ModelState.AddModelError(DisplayNames.Title,
                        _sharedLocalizer[(string)TempData[TitleErrorMessage]]);
                }
                if (TempData.ContainsKey(AuthorErrorMessage))
                {
                    ModelState.AddModelError(DisplayNames.Author,
                        _sharedLocalizer[(string)TempData[AuthorErrorMessage]]);
                }
                if (TempData.ContainsKey(SecretCodeMessage))
                {
                    viewModel.SecretCodeMessage
                        = _sharedLocalizer[(string)TempData[SecretCodeMessage]];
                }

                if (user.DailyPersonalGoal.HasValue)
                {
                    var programDays = (int)Math.Ceiling((
                        site.ProgramEnds.Value - site.ProgramStarts.Value).TotalDays);
                    viewModel.TotalProgramGoal = programDays * user.DailyPersonalGoal.Value;
                    viewModel.ActivityEarned = await _activityService.GetActivityEarnedAsync();
                }
                else
                {
                    viewModel.TotalProgramGoal = program.AchieverPointAmount;
                    viewModel.ActivityEarned = user.PointsEarned;
                    viewModel.ProgressMessage
                        = _sharedLocalizer[Annotations.Info.Goal, program.AchieverPointAmount];
                }
                viewModel.PercentComplete = Math.Min(
                        (int)(viewModel.ActivityEarned * 100 / viewModel.TotalProgramGoal), 100);

                var userVendorCode = await _vendorCodeService.GetUserVendorCodeAsync(user.Id);
                if (userVendorCode?.CanBeDonated == true && userVendorCode.IsDonated == null
                    && (!userVendorCode.ExpirationDate.HasValue
                        || userVendorCode.ExpirationDate.Value > _dateTimeProvider.Now))
                {
                    viewModel.HasPendingDonationQuestion = true;
                    viewModel.VendorCodeExpiration = userVendorCode.ExpirationDate;
                }

                return View(ViewTemplates.Dashboard, viewModel);
            }
            else
            {
                return await ShowLandingPageAsync(site, GetSiteStage());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReminder(BeforeRegistrationViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Email))
            {
                await _emailReminderService
                    .AddEmailReminderAsync(viewModel.Email, viewModel.SignUpSource);
                ShowAlertInfo(_sharedLocalizer[Annotations.Info.LetYouKnowWhen], "envelope");
            }
            return RedirectToAction(nameof(Index));
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
            return await ShowExitPageAsync(GetSiteStage());
        }

        public IActionResult LogActivity()
        {
            return RedirectToAction(nameof(Index));
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
                TempData[ActivityErrorMessage]
                    = _sharedLocalizer[Annotations.Validate.WholeNumber].Value;
            }
            if (!ModelState.IsValid)
            {
                valid = false;
                if (ModelState[DisplayNames.Title].Errors.Count > 0)
                {
                    TempData[TitleErrorMessage]
                        = ModelState[DisplayNames.Title].Errors.First().ErrorMessage;
                }
                if (ModelState[DisplayNames.Author].Errors.Count > 0)
                {
                    TempData[AuthorErrorMessage] = ModelState[DisplayNames.Author]
                        .Errors
                        .First()
                        .ErrorMessage;
                }
            }
            if (string.IsNullOrWhiteSpace(viewModel.Title)
                && !string.IsNullOrWhiteSpace(viewModel.Author))
            {
                valid = false;
                TempData[TitleErrorMessage]
                    = _sharedLocalizer[Annotations.Validate.BookTitle].Value;
            }
            if (!valid)
            {
                TempData[ModelData] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel);
            }
            else
            {
                var book = new Book
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
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.CouldNotLog,
                        gex.Message]);
                }
            }
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCarouselDescription(int id)
        {
            string carouselItemDescription = null;
            try
            {
                carouselItemDescription = await _carouselService.GetItemDescriptionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Request for carousel item {id} description failed: {message}",
                    id,
                    ex.Message);
            }

            if (string.IsNullOrEmpty(carouselItemDescription))
            {
                return NotFound();
            }
            else
            {
                return Ok(CommonMark.CommonMarkConverter.Convert(carouselItemDescription));
            }
        }

        public async Task<IActionResult> Unsubscribe(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    await _emailManagementService.TokenUnsubscribe(id);
                    ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.Unsubscribed,
                        HttpContext.Items[ItemKey.SiteName]?.ToString()]);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(gex.Message);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> ShowExitPageAsync(SiteStage siteStage,
            bool performLogout = true)
        {
            string siteName = HttpContext.Items[ItemKey.SiteName]?.ToString();
            PageTitle = _sharedLocalizer[Annotations.Title.SeeYouSoon, siteName];
            ViewResult viewResult;
            var exitPageViewModel = new ExitPageViewModel
            {
                Branch = await _userService.GetUsersBranch(GetActiveUserId())
            };
            switch (siteStage)
            {
                case SiteStage.BeforeRegistration:
                    viewResult = View(ViewTemplates.ExitBeforeRegistration, exitPageViewModel);
                    break;
                case SiteStage.RegistrationOpen:
                    viewResult = View(ViewTemplates.ExitRegistrationOpen, exitPageViewModel);
                    break;
                case SiteStage.ProgramEnded:
                    viewResult = View(ViewTemplates.ExitProgramEnded, exitPageViewModel);
                    break;
                case SiteStage.AccessClosed:
                    viewResult = View(ViewTemplates.ExitAccessClosed, exitPageViewModel);
                    break;
                default:
                    viewResult = View(ViewTemplates.ExitProgramOpen, exitPageViewModel);
                    break;
            }
            if (performLogout)
            {
                await LogoutUserAsync();
            }
            return viewResult;
        }

        private async Task<IActionResult> ShowLandingPageAsync(Site site, SiteStage siteStage)
        {
            string siteName = HttpContext.Items[ItemKey.SiteName]?.ToString();
            PageTitle = siteName;
            switch (siteStage)
            {
                case SiteStage.BeforeRegistration:
                    var viewModel = new BeforeRegistrationViewModel
                    {
                        SignUpSource = nameof(SiteStage.BeforeRegistration),
                        SiteName = siteName
                    };
                    if (site != null)
                    {
                        viewModel.CollectEmail = await _siteLookupService
                            .GetSiteSettingBoolAsync(site.Id,
                                SiteSettingKey.Users.CollectPreregistrationEmails);
                        if (site.RegistrationOpens != null)
                        {
                            viewModel.RegistrationOpens
                                = ((DateTime)site.RegistrationOpens).ToString("D");
                            PageTitle = _sharedLocalizer[Annotations.Title.RegistrationOpens,
                                siteName,
                                viewModel.RegistrationOpens];
                        }
                    }
                    return View(ViewTemplates.BeforeRegistration, viewModel);
                case SiteStage.RegistrationOpen:
                    PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, siteName];
                    return View(ViewTemplates.RegistrationOpen, siteName);
                case SiteStage.ProgramEnded:
                    return View(ViewTemplates.ProgramEnded, siteName);
                case SiteStage.AccessClosed:
                    var acViewModel = new AccessClosedViewModel
                    {
                        SignUpSource = nameof(SiteStage.AccessClosed),
                    };
                    if (site != null)
                    {
                        acViewModel.SiteName = site.Name;
                        acViewModel.CollectEmail = await _siteLookupService
                            .GetSiteSettingBoolAsync(site.Id,
                                SiteSettingKey.Users.CollectAccessClosedEmails);
                    }
                    return View(ViewTemplates.AccessClosed, acViewModel);
                default:
                    PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, siteName];
                    return View(ViewTemplates.ProgramOpen, siteName);
            }
        }

        private SiteStage ParseStage(string id)
        {
            switch (id)
            {
                case nameof(SiteStage.BeforeRegistration):
                    return SiteStage.BeforeRegistration;
                case nameof(SiteStage.RegistrationOpen):
                    return SiteStage.RegistrationOpen;
                case nameof(SiteStage.ProgramEnded):
                    return SiteStage.ProgramEnded;
                case nameof(SiteStage.AccessClosed):
                    return SiteStage.AccessClosed;
                default:
                    return SiteStage.ProgramOpen;
            }
        }

        public async Task<IActionResult> PreviewLanding(string id)
        {
            if (UserHasPermission(Permission.AccessMissionControl))
            {
                SiteStage stage = ParseStage(id);
                return await ShowLandingPageAsync(await GetCurrentSiteAsync(), stage);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> PreviewExit(string id)
        {
            if (UserHasPermission(Permission.AccessMissionControl))
            {
                SiteStage stage = ParseStage(id);
                return await ShowExitPageAsync(stage, false);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
