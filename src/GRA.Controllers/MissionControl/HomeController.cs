using GRA.Controllers.ViewModel.MissionControl.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class HomeController : Base.MCController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly ReportService _reportService;
        private readonly SampleDataService _sampleDataService;
        private readonly UserService _userService;
        private readonly SiteService _siteService;
        public HomeController(ILogger<HomeController> logger,
            AuthenticationService authenticationService,
            ReportService reportService,
            SampleDataService sampleDataService,
            UserService userService,
            SiteService siteService,
            ServiceFacade.Controller context)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _reportService = Require.IsNotNull(reportService, nameof(reportService));
            _sampleDataService = Require.IsNotNull(sampleDataService,
                nameof(sampleDataService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Mission Control";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = "/MissionControl"
                });
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code
                return View("AuthorizationCode");
            }
            Site site = await GetCurrentSiteAsync(sitePath);
            PageTitle = $"Mission Control: {site.Name}";

            // show the at-a-glance report
            int currentUserBranchId = GetId(ClaimType.BranchId);

            var siteStatus = await _reportService.GetCurrentStats(new StatusSummary());
            var branchStatus = await _reportService.GetCurrentStats(new StatusSummary
            {
                BranchId = currentUserBranchId
            });
            var branchName = await _siteService.GetBranchName(GetId(ClaimType.BranchId));

            return View(new AtAGlanceViewModel
            {
                SiteStatus = siteStatus,
                FilteredBranchDescription = $"Your branch ({branchName})",
                FilteredStatus = branchStatus
            });
        }

        [HttpGet]
        public IActionResult AuthorizationCode()
        {
            return View();
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
                    ReturnUrl = "/MissionControl"
                });
            }

            string role
                = await _userService.ActivateAuthorizationCode(viewmodel.AuthorizationCode);

            if (!string.IsNullOrEmpty(role))
            {
                var auth = await _authenticationService
                    .RevalidateUserAsync(GetId(ClaimType.UserId));
                auth.AuthenticationMessage = $"Code applied, you are now a member of the role: {role}.";
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
                return View("AuthorizationCode");
            }
        }

        public async Task<IActionResult> LoadSampleData()
        {
            await _sampleDataService.InsertSampleData(GetId(ClaimType.UserId));
            AlertSuccess = "Inserted sample data.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToRoute(new { area = string.Empty, action = "Index" });
        }
    }
}
