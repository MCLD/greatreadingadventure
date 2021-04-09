using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly MailService _mailService;
        private readonly NewsService _newsService;
        private readonly ReportService _reportService;
        private readonly UserService _userService;
        private readonly SiteService _siteService;

        public static string Name { get { return "Home"; } }

        public HomeController(ILogger<HomeController> logger,
            AuthenticationService authenticationService,
            MailService mailService,
            NewsService newsService,
            ReportService reportService,
            UserService userService,
            SiteService siteService,
            ServiceFacade.Controller context)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));

            PageTitle = "Mission Control";
        }

        public async Task<IActionResult> Index(int? category, int page = 1)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                return RedirectToSignIn();
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code

                return RedirectToAction(nameof(AuthorizationCode));
            }

            // change user to default language
            if (Culture.DefaultName != _userContextProvider.GetCurrentCulture().Name)
            {
                AlertInfo = $"Language changed to <strong>{Culture.DefaultCulture.DisplayName}</strong> for <strong>Mission Control</strong> <span class=\"fas fa-rocket\"></span>.";
                return RedirectToAction(nameof(Index), new { culture = Culture.DefaultName });
            }

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
                if (paginateModel.PastMaxPage)
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
                foreach (var item in viewModel.NewsCategories)
                {
                    item.IsNew = _newsService.WithinTimeFrame(item.LastPostDate, 7);
                }
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
        public IActionResult AuthorizationCode()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                var siteStage = GetSiteStage();
                if (siteStage == SiteStage.ProgramOpen || siteStage == SiteStage.RegistrationOpen)
                {
                    ShowAlertWarning(
                        _sharedLocalizer[Annotations.Validate.AuthorizationCodeWarning,
                        Url.Action(nameof(JoinController.AuthorizationCode),
                            JoinController.Name)]);
                    return RedirectToSignIn();
                }
                else
                {
                    return RedirectToAction(nameof(JoinController.AuthorizationCode),
                        JoinController.Name);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AuthorizationCode(AuthorizationCodeViewModel viewmodel)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                return RedirectToSignIn();
            }

            if (viewmodel != null && ModelState.IsValid)
            {
                string sanitized = viewmodel.AuthorizationCode.Trim().ToLowerInvariant();

                try
                {
                    string role
                        = await _userService.ActivateAuthorizationCode(sanitized);

                    if (!string.IsNullOrEmpty(role))
                    {
                        var auth = await _authenticationService
                            .RevalidateUserAsync(GetId(ClaimType.UserId));
                        // TODO globalize
                        auth.Message = $"Code applied, you are a member of the role: {role}.";
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
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUser();
            }
            return RedirectToRoute(new { area = string.Empty, action = nameof(HomeController.Index) });
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<JsonResult> GetUnreadMailCount()
        {
            var unreadCount = await _mailService.GetAdminUnreadCountAsync();
            return Json(unreadCount);
        }

        [HttpPost]
        public IActionResult Job(string id)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                return RedirectToSignIn();
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code

                return RedirectToAction(nameof(AuthorizationCode));
            }

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                JobToken = id
            });
        }
    }
}
