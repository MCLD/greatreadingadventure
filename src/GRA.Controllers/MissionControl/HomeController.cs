using GRA.Controllers.ViewModel.MissionControl;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class HomeController : Base.Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SampleDataService _configurationService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,
            SampleDataService configurationService,
            UserService userService,
            ServiceFacade.Controller context)
            : base(context)
        {
            this._logger = Require.IsNotNull(logger, nameof(logger));
            this._configurationService = Require.IsNotNull(configurationService,
                nameof(configurationService));
            this._userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Mission Control";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new { area = string.Empty, controller = "SignIn", ReturnUrl = "/MissionControl" });
            }

            if (!UserHasPermission(Permission.AccessMissionControl))
            {
                // not authorized for Mission Control, redirect to authorization code
                return View("AuthorizationCode");
            }
            Site site = await GetCurrentSite(sitePath);
            PageTitle = $"Mission Control: {site.Name}";
            return View();
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
                return RedirectToRoute(new { area = string.Empty, controller = "SignIn", ReturnUrl = "/MissionControl" });
            }

            string role
                = await _userService.ActivateAuthorizationCode(viewmodel.AuthorizationCode);

            if (!string.IsNullOrEmpty(role))
            {
                AlertSuccess = $"Code applied, you are now a member of the role: <strong>{role}</strong>. Please log in again to access your new rights.";
                await LogoutUserAsync();
                return RedirectToRoute(new { area = string.Empty, controller = "SignIn", action = "Index"});
            }
            else
            {
                AlertDanger = "Invalid code. This request was logged.";
                return View("AuthorizationCode");
            }
        }

        public async Task<IActionResult> LoadSampleData()
        {
            await _configurationService.InsertSampleData(GetId(ClaimType.UserId));
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
