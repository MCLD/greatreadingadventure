using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stubble.Core.Builders;

namespace GRA.Controllers
{
    public class HomeController : Base.UserController
    {
        private const string ActivityErrorMessage = "ActivityErrorMessage";
        private const string AuthorErrorMessage = "AuthorErrorMessage";
        private const int BadgesToDisplay = 6;
        private const string DefaultBannerFilename = "gra-forest.jpg";
        private const string LatestBook = "LatestBook";
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
        private readonly ExitLandingService _exitLandingService;
        private readonly LanguageService _languageService;
        private readonly ILogger<HomeController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SegmentService _segmentService;
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
            ExitLandingService exitLandingService,
            LanguageService languageService,
            PerformerSchedulingService performerSchedulingService,
            SegmentService segmentService,
            SiteService siteService,
            SocialService socialService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(activityService);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(carouselService);
            ArgumentNullException.ThrowIfNull(dailyLiteracyTipService);
            ArgumentNullException.ThrowIfNull(dashboardContentService);
            ArgumentNullException.ThrowIfNull(emailManagementService);
            ArgumentNullException.ThrowIfNull(emailReminderService);
            ArgumentNullException.ThrowIfNull(eventService);
            ArgumentNullException.ThrowIfNull(exitLandingService);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(performerSchedulingService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(socialService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _activityService = activityService;
            _avatarService = avatarService;
            _carouselService = carouselService;
            _dailyLiteracyTipService = dailyLiteracyTipService;
            _dashboardContentService = dashboardContentService;
            _emailManagementService = emailManagementService;
            _emailReminderService = emailReminderService;
            _eventService = eventService;
            _exitLandingService = exitLandingService;
            _languageService = languageService;
            _logger = logger;
            _performerSchedulingService = performerSchedulingService;
            _segmentService = segmentService;
            _siteService = siteService;
            _socialService = socialService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;
        }

        public static string Name
        { get { return "Home"; } }

        [HttpPost]
        public async Task<IActionResult> AddReminder(LandingPageViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel?.Email))
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
            catch (GraException gex)
            {
                _logger.LogWarning(gex,
                    "Request for carousel item {id} description failed: {message}",
                    id,
                    gex.Message);
            }

            return string.IsNullOrEmpty(carouselItemDescription)
                ? NotFound()
                : Ok(CommonMark.CommonMarkConverter.Convert(carouselItemDescription));
        }

        [HttpPost]
        public async Task<IActionResult> GetIcs()
        {
            var site = await GetCurrentSiteAsync();
            var filename = new string(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());
            var siteLink = await _siteLookupService.GetSiteLinkAsync(site.Id);
            var calendarBytes = await _siteService.GetIcsFile(siteLink.ToString());
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
                    new UserLogFilter(null, BadgesToDisplay)
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
                    ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural,
                    DisableSecretCode
                        = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                    IsPerformerScheduling = GetSiteStage() == SiteStage.BeforeRegistration
                        && User.HasClaim(ClaimType.Permission,
                            nameof(Permission.AccessPerformerRegistration))
                        && !User.HasClaim(ClaimType.Permission,
                            nameof(Permission.AccessMissionControl)),
                    SingleEvent = pointTranslation.IsSingleEvent,
                    SiteStage = SiteStage.Unknown,
                    UpcomingStreams = await _eventService.GetUpcomingStreamListAsync(),
                    User = user,
                    UserLogs = userLogs.Data
                };

                var latestBookCookie = Request.Cookies[$"{GetActiveUserId()}{LatestBook}"];
                if (latestBookCookie != null)
                {
                    var bookData = latestBookCookie.Split(";;");

                    viewModel.Title = bookData[0];
                    viewModel.Author = bookData.Length == 2 ? bookData[1] : null;
                }

                if (HttpContext.Items.TryGetValue(ItemKey.SiteStage, out var stage))
                {
                    viewModel.SiteStage = (SiteStage)stage;
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
                            var pathElements = new[] {
                                $"site{site.Id}",
                                "dailyimages",
                                $"dailyliteracytip{program.DailyLiteracyTipId}",
                                image.Name + image.Extension
                            };

                            var imagePath = string.Join('/', pathElements);

                            if (System.IO.File.Exists(_pathResolver
                                .ResolveContentFilePath(imagePath)))
                            {
                                var dailyLiteracyTip = await _dailyLiteracyTipService
                                    .GetByIdAsync(program.DailyLiteracyTipId.Value);
                                viewModel.DailyImageLarge = dailyLiteracyTip.IsLarge;
                                viewModel.DailyImageMessage = dailyLiteracyTip.Message;
                                viewModel.DailyImagePath
                                    = _pathResolver.ResolveContentPath(imagePath);
                                viewModel.Day = day.Value;
                            }
                        }
                    }
                }

                if (program.ButtonSegmentId.HasValue)
                {
                    var culture = _userContextProvider.GetCurrentCulture();
                    var currentLanguageId = await _languageService.GetLanguageIdAsync(culture.Name);
                    viewModel.ButtonText = await _segmentService.GetDbTextAsync(
                        program.ButtonSegmentId.Value,
                        currentLanguageId);
                }

                if (!string.IsNullOrEmpty(program.DashboardAlert?.Trim()))
                {
                    viewModel.DashboardAlert = program.DashboardAlert;
                    viewModel.DashboardAlertType = program.DashboardAlertType;
                }

                if (TempData.ContainsKey(TempDataKey.UserJoined))
                {
                    viewModel.FirstTimeParticipant = user.IsFirstTime;
                    viewModel.ProgramName = program.Name;
                    viewModel.UserJoined = true;
                }
                TempData.Remove(TempDataKey.UserJoined);

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

                if (TempData.TryGetValue(ModelData, out object modelData))
                {
                    var model = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<DashboardViewModel>((string)modelData);
                    viewModel.ActivityAmount = model.ActivityAmount;
                    viewModel.Title = model.Title;
                    viewModel.Author = model.Author;
                }
                if (TempData.TryGetValue(ActivityErrorMessage, out object activityErrorMessage))
                {
                    ModelState.AddModelError("ActivityAmount",
                        _sharedLocalizer[(string)activityErrorMessage]);
                    viewModel.ActivityAmount = null;
                }
                if (TempData.TryGetValue(TitleErrorMessage, out object titleErrorMessage))
                {
                    ModelState.AddModelError(DisplayNames.Title,
                        _sharedLocalizer[(string)titleErrorMessage]);
                }
                if (TempData.TryGetValue(AuthorErrorMessage, out object authorErrorMessage))
                {
                    ModelState.AddModelError(DisplayNames.Author,
                        _sharedLocalizer[(string)authorErrorMessage]);
                }
                if (TempData.TryGetValue(SecretCodeMessage, out object secretCodeMessage))
                {
                    viewModel.SecretCodeMessage
                        = _sharedLocalizer[(string)secretCodeMessage];
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

                var (siteReadingGoalSet, siteReadingGoal)
                    = await _siteLookupService.GetSiteSettingIntAsync(GetCurrentSiteId(),
                        SiteSettingKey.Site.ReadingGoalInMinutes);

                if (siteReadingGoalSet)
                {
                    viewModel.SiteReadingGoal = siteReadingGoal;
                    viewModel.TotalSiteActivity
                        = await _activityService.GetSiteActivityEarnedAsync();
                    viewModel.SiteActivityPercentComplete = Math.Min(
                        (int)(viewModel.TotalSiteActivity * 100.0 / viewModel.SiteReadingGoal), 100);
                }

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
            if (viewModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

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

                if (viewModel.Title != null)
                {
                    Response.Cookies.Append($"{GetActiveUserId()}{LatestBook}", $"{viewModel.Title};;{viewModel.Author ?? ""}");
                }

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
            if (viewModel == null)
            {
                TempData[SecretCodeMessage] = "There was an error with that code.";
            }
            else
            {
                try
                {
                    await _activityService.LogSecretCodeAsync(GetActiveUserId(),
                        viewModel.SecretCode);
                }
                catch (GraException gex)
                {
                    TempData[SecretCodeMessage] = gex.Message;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PreviewExit(string id)
        {
            return UserHasPermission(Permission.AccessMissionControl)
                ? await ShowExitPageAsync(ParseStage(id))
                : RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PreviewLanding(string id)
        {
            return UserHasPermission(Permission.AccessMissionControl)
                ? await ShowLandingPageAsync(await GetCurrentSiteAsync(), ParseStage(id))
                : RedirectToAction(nameof(Index));
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

            var culture = _userContextProvider.GetCurrentCulture();
            var currentLanguageId = await _languageService.GetLanguageIdAsync(culture.Name);

            var exitLandingDetails = await _exitLandingService.GetExitLandingDetailsAsync(siteStage,
                currentLanguageId);

            var exitPageViewModel = new ExitPageViewModel
            {
                BannerAltText = _sharedLocalizer[Annotations.Home.BannerAltTag],
                BannerImagePath = $"/{_pathResolver.ResolveContentPath(DefaultBannerFilename)}",
                LeftMessage = CommonMark.CommonMarkConverter
                    .Convert(exitLandingDetails.ExitLeftMessage)
            };

            try
            {
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
            var currentLanguageId = await _languageService.GetLanguageIdAsync(culture.Name);

            var exitLandingDetails = await _exitLandingService.GetExitLandingDetailsAsync(siteStage,
                currentLanguageId);

            var stubble = new StubbleBuilder().Build();
            var dataHash = new Dictionary<string, string>
            {
                {nameof(Tags.ExitLandingTags.Sitename), siteName },
                {nameof(Tags.ExitLandingTags.RegistrationOpens),
                    ((DateTime)site.RegistrationOpens).ToString("D", culture) }
            };

            var viewmodel = new LandingPageViewModel
            {
                BannerAltText = _sharedLocalizer[Annotations.Home.BannerAltTag],
                BannerImagePath = $"/{_pathResolver.ResolveContentPath(DefaultBannerFilename)}",
                CenterMessage = CommonMark.CommonMarkConverter.Convert(
                    await stubble.RenderAsync(exitLandingDetails.LandingCenterMessage, dataHash)),
                LeftMessage = CommonMark.CommonMarkConverter.Convert(
                    await stubble.RenderAsync(exitLandingDetails.LandingLeftMessage, dataHash)),
                RightMessage = CommonMark.CommonMarkConverter.Convert(
                    await stubble.RenderAsync(exitLandingDetails.LandingRightMessage, dataHash)),
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
