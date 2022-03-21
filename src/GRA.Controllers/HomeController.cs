using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class HomeController : Base.UserController
    {
        private const string ActivityErrorMessage = "ActivityErrorMessage";
        private const string AuthorErrorMessage = "AuthorErrorMessage";
        private const int BadgesToDisplay = 6;
        private const string ModelData = "ModelData";
        private const string SecretCodeMessage = "SecretCodeMessage";
        private const string TitleErrorMessage = "TitleErrorMessage";
        private readonly ActivityService _activityService;
        private readonly AvatarService _avatarService;
        private readonly CarouselService _carouselService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly DashboardContentService _dashboardContentService;
        private readonly EmailManagementService _emailManagementService;
        private readonly EmailReminderService _emailReminderService;
        private readonly EventService _eventService;
        private readonly LanguageService _languageService;
        private readonly ILogger<HomeController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteService _siteService;
        private readonly SocialService _socialService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AvatarService avatarService,
            CarouselService carouselService,
            DailyLiteracyTipService dailyLiteracyTipService,
            DashboardContentService dashboardContentService,
            EmailManagementService emailManagementService,
            EmailReminderService emailReminderService,
            EventService eventService,
            LanguageService languageService,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService,
            SocialService socialService,
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
            _eventService = eventService
                ?? throw new ArgumentNullException(nameof(eventService));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _socialService = socialService
                ?? throw new ArgumentNullException(nameof(socialService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
        }

        public static string Name
        { get { return "Home"; } }

        [HttpPost]
        public async Task<IActionResult> AddReminder(LandingPageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Email))
            {
                var currentCultureName = _userContextProvider.GetCurrentCulture()?.Name;
                var currentLanguageId = await _languageService
                    .GetLanguageIdAsync(currentCultureName);
                await _emailReminderService
                    .AddEmailReminderAsync(viewModel.Email,
                    viewModel.SignUpSource,
                    currentLanguageId);
                ShowAlertInfo(_sharedLocalizer[Annotations.Info.LetYouKnowWhen], "envelope");
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

        [HttpPost]
        public async Task<IActionResult> GetIcs()
        {
            var site = await GetCurrentSiteAsync();
            var filename = new string(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var calendarBytes = await _siteService.GetIcsFile(siteUrl);
            return File(calendarBytes, "text/calendar", $"{filename}.ics");
        }

        public async Task<IActionResult> Goodbye(int id)
        {
            return await ShowExitPageAsync(GetSiteStage(), id);
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

                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                var userLogs = await _userService.GetPaginatedUserHistoryAsync(user.Id,
                    new UserLogFilter(take: BadgesToDisplay)
                    {
                        HasBadge = true
                    });

                foreach (var userLog in userLogs.Data)
                {
                    userLog.BadgeFilename
                        = _pathResolver.ResolveContentPath(userLog.BadgeFilename);
                }

                var pointTranslation = await _activityService.GetUserPointTranslationAsync();
                var viewModel = new DashboardViewModel
                {
                    User = user,
                    SingleEvent = pointTranslation.IsSingleEvent,
                    ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural,
                    UserLogs = userLogs.Data,
                    DisableSecretCode
                        = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                    UpcomingStreams = await _eventService
                        .GetUpcomingStreamListAsync()
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
                                $"{image.Name}{image.Extension}");

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
                    viewModel.HasPendingVendorCodeQuestion = true;
                    viewModel.VendorCodeExpiration = userVendorCode.ExpirationDate;
                }

                return View(ViewTemplates.Dashboard, viewModel);
            }
            else
            {
                return await ShowLandingPageAsync(site, GetSiteStage());
            }
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

        public async Task<IActionResult> PreviewExit(string id)
        {
            if (UserHasPermission(Permission.AccessMissionControl))
            {
                SiteStage stage = ParseStage(id);
                return await ShowExitPageAsync(stage);
            }
            else
            {
                return RedirectToAction(nameof(Index));
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

        [PreventQuestionnaireRedirect]
        public async Task<IActionResult> Signout()
        {
            var id = UserClaim(ClaimType.BranchId);

            await LogoutUser();

            return RedirectToAction(nameof(Goodbye), new { id });
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

        private static SiteStage ParseStage(string siteStageText)
        {
            return Enum.TryParse(siteStageText, out SiteStage siteStage)
                ? siteStage
                : SiteStage.ProgramOpen;
        }

        private async Task<IActionResult> ShowExitPageAsync(SiteStage siteStage,
                    int? branchId = null)
        {
            string siteName = HttpContext.Items[ItemKey.SiteName]?.ToString();
            PageTitle = _sharedLocalizer[Annotations.Title.SeeYouSoon, siteName];

            var exitPageViewModel = new ExitPageViewModel();

            try
            {
                var culture = _userContextProvider.GetCurrentCulture();
                exitPageViewModel.Social = await _socialService.GetAsync(culture.Name);
            }
            catch (GraException ex)
            {
                _logger.LogInformation(ex,
                    "Unable to populate social card for exit page: {ErrorMessage}",
                    ex.Message);
            }

            if (branchId == null)
            {
                try
                {
                    exitPageViewModel.Branch
                        = await _userService.GetUsersBranch(GetActiveUserId());
                }
                catch (GraException) { }
            }

            if (exitPageViewModel.Branch == null && branchId.HasValue)
            {
                try
                {
                    exitPageViewModel.Branch
                        = await _siteService.GetBranchByIdAsync(branchId.Value);
                }
                catch (GraException) { }
            }

            return siteStage switch
            {
                SiteStage.BeforeRegistration =>
                    View(ViewTemplates.ExitBeforeRegistration, exitPageViewModel),
                SiteStage.RegistrationOpen =>
                    View(ViewTemplates.ExitRegistrationOpen, exitPageViewModel),
                SiteStage.ProgramEnded => View(ViewTemplates.ExitProgramEnded, exitPageViewModel),
                SiteStage.AccessClosed => View(ViewTemplates.ExitAccessClosed, exitPageViewModel),
                _ => View(ViewTemplates.ExitProgramOpen, exitPageViewModel),
            };
        }

        private async Task<IActionResult> ShowLandingPageAsync(Site site, SiteStage siteStage)
        {
            string siteName = HttpContext.Items[ItemKey.SiteName]?.ToString();
            PageTitle = siteName;

            var culture = _userContextProvider.GetCurrentCulture();

            // social
            var viewmodel = new LandingPageViewModel
            {
                SiteName = siteName,
                Social = await _socialService.GetAsync(culture.Name)
            };

            switch (siteStage)
            {
                case SiteStage.BeforeRegistration:
                    viewmodel.SignUpSource = nameof(SiteStage.BeforeRegistration);
                    if (site != null)
                    {
                        viewmodel.CollectEmail = await _siteLookupService
                            .GetSiteSettingBoolAsync(site.Id,
                                SiteSettingKey.Users.CollectPreregistrationEmails);

                        if (site.RegistrationOpens != null)
                        {
                            viewmodel.RegistrationOpens
                                = ((DateTime)site.RegistrationOpens).ToString("D",
                                    culture);
                            PageTitle = _sharedLocalizer[Annotations.Title.RegistrationOpens,
                                siteName,
                                viewmodel.RegistrationOpens];
                        }
                    }
                    return View(ViewTemplates.BeforeRegistration, viewmodel);

                case SiteStage.RegistrationOpen:
                    PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, siteName];
                    return View(ViewTemplates.RegistrationOpen, viewmodel);

                case SiteStage.ProgramEnded:
                    return View(ViewTemplates.ProgramEnded, viewmodel);

                case SiteStage.AccessClosed:
                    viewmodel.SignUpSource = nameof(SiteStage.AccessClosed);
                    if (site != null)
                    {
                        viewmodel.CollectEmail = await _siteLookupService
                            .GetSiteSettingBoolAsync(site.Id,
                                SiteSettingKey.Users.CollectAccessClosedEmails);
                    }
                    return View(ViewTemplates.AccessClosed, viewmodel);

                default:
                    PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, siteName];
                    return View(ViewTemplates.ProgramOpen, viewmodel);
            }
        }
    }
}
