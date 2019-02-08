using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Controllers.ViewModel.MissionControl.Home;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class HomeController : Base.MCController
    {
        private const int PostsPerPage = 5;

        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly LanguageService _languageService;
        private readonly MailService _mailService;
        private readonly NewsService _newsService;
        private readonly ReportService _reportService;
        private readonly UserService _userService;
        private readonly SiteService _siteService;

        private readonly ICodeSanitizer _codeSanitizer;

        public HomeController(ILogger<HomeController> logger,
            AuthenticationService authenticationService,
            LanguageService languageService,
            MailService mailService,
            NewsService newsService,
            ReportService reportService,
            UserService userService,
            SiteService siteService,
            ServiceFacade.Controller context,
            ICodeSanitizer codeSanitizer)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _codeSanitizer = codeSanitizer
                ?? throw new ArgumentNullException(nameof(codeSanitizer));

            PageTitle = "Mission Control";
        }

        public async Task<IActionResult> Index(int? category, int page = 1)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = Url.Action()
                });
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code

                return RedirectToAction(nameof(AuthorizationCode));
            }

            await _languageService.SyncLanguagesAsync(GetActiveUserId());

            Site site = await GetCurrentSiteAsync();

            var viewModel = new AtAGlanceViewModel
            {
                AtAGlanceReport = await GetAtAGlanceAsync(),
                ShowPosts = await _newsService.AnyPublishedPostsAsync()
            };

            if (viewModel.ShowPosts)
            {
                var filter = new NewsFilter(page, PostsPerPage)
                {
                    IsActive = true
                };

                if (category.HasValue)
                {
                    filter.CategoryIds = new List<int> { category.Value };
                }
                else
                {
                    filter.DefaultCategory = true;
                }

                var postList = await _newsService.GetPaginatedPostListAsync(filter);

                var paginateModel = new PaginateViewModel
                {
                    ItemCount = postList.Count,
                    CurrentPage = page,
                    ItemsPerPage = filter.Take.Value
                };
                if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = paginateModel.LastPage ?? 1
                        });
                }

                foreach (var post in postList.Data)
                {
                    post.Content = CommonMark.CommonMarkConverter.Convert(post.Content);
                    post.CreatedByName = await _userService.GetUsersNameByIdAsync(post.CreatedBy);
                }

                var user = await _userService.GetDetails(GetId(ClaimType.UserId));

                viewModel.IsNewsSubscribed = user.IsNewsSubscribed;
                viewModel.NewsPosts = postList.Data;
                viewModel.NewsCategories = await _newsService.GetAllCategoriesAsync();
                viewModel.PaginateModel = paginateModel;
                viewModel.SiteAdministratorEmail = site.FromEmailAddress;
            }

            PageTitle = $"Mission Control: {site.Name}";

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.AccessMissionControl)]
        public async Task<JsonResult> NewsSubscribe(bool subscribe)
        {
            var success = false;
            var message = "";
            try
            {
                await _userService.UserNewsSubscribe(subscribe);
                success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating new subscription status for user {GetId(ClaimType.UserId)}", ex);
                message = "Error updating news subscription";
            }

            return Json(new
            {
                success,
                message
            });
        }

        [HttpGet]
        [Authorize(Policy = Policy.AccessMissionControl)]
        public async Task<JsonResult> AtAGlanceReport()
        {
            var atAGlance = await GetAtAGlanceAsync();
            return Json(atAGlance);
        }

        [Authorize(Policy = Policy.AccessMissionControl)]
        private async Task<AtAGlanceReport> GetAtAGlanceAsync()
        {
            int currentUserBranchId = GetId(ClaimType.BranchId);

            var siteStatus = await _reportService.GetCurrentStatsAsync(new ReportCriterion());
            var branchStatus = await _reportService.GetCurrentStatsAsync(new ReportCriterion
            {
                BranchId = currentUserBranchId
            });

            return new AtAGlanceReport
            {
                FilteredBranchDescription = await _siteService.GetBranchName(currentUserBranchId),
                SiteStatus = siteStatus,
                FilteredStatus = branchStatus,
                LatestNewsId = await _newsService.GetLatestNewsIdAsync()
            };
        }

        [HttpGet]
        public async Task<IActionResult> AuthorizationCode()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = Url.Action()
                });
            }

            var site = await GetCurrentSiteAsync();
            string siteLogoUrl = site.SiteLogoUrl
                ?? Url.Content(Defaults.SiteLogoPath);

            return View(new AuthorizationCodeViewModel
            {
                SiteLogoUrl = siteLogoUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> AuthorizationCode(AuthorizationCodeViewModel viewmodel)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = Url.Action()
                });
            }

            if (ModelState.IsValid)
            {
                string sanitized = _codeSanitizer.Sanitize(viewmodel.AuthorizationCode, 255);

                try
                {
                    string role
                        = await _userService.ActivateAuthorizationCode(sanitized);

                    if (!string.IsNullOrEmpty(role))
                    {
                        var auth = await _authenticationService
                            .RevalidateUserAsync(GetId(ClaimType.UserId));
                        auth.AuthenticationMessage = $"Code applied, you are now a member of the role: <strong>{role}</strong>.";
                        await LoginUserAsync(auth);
                        return RedirectToRoute(new
                        {
                            area = "MissionControl",
                            controller = "Home",
                            action = "Index"
                        });
                    }
                    else
                    {
                        ShowAlertDanger("Invalid code. This request was logged.");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to activate code: ", gex);
                }
            }
            var site = await GetCurrentSiteAsync();
            string siteLogoUrl = site.SiteLogoUrl
                ?? Url.Content(Defaults.SiteLogoPath);

            return View(new AuthorizationCodeViewModel
            {
                SiteLogoUrl = siteLogoUrl
            });
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { area = string.Empty, action = "Index" });
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<JsonResult> GetUnreadMailCount()
        {
            var unreadCount = await _mailService.GetAdminUnreadCountAsync();
            return Json(unreadCount);
        }
    }
}
