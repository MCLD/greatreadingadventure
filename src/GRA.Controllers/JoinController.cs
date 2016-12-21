using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.ViewModel.Join;
using Microsoft.AspNetCore.Mvc.Rendering;
using GRA.Domain.Service;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    public class JoinController : Base.UserController
    {
        private readonly ILogger<JoinController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly AuthenticationService _authenticationService;
        private readonly UserService _userService;
        private readonly SiteService _siteService;
        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            UserService userService,
            SiteService siteService)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Join";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSiteAsync();
            var siteStage = GetSiteStage();
            if (siteStage == SiteStage.BeforeRegistration)
            {
                if (site.RegistrationOpens.HasValue)
                {
                    AlertInfo = $"You can join {site.Name} on {site.RegistrationOpens.Value.ToString("d")}";
                }
                else
                {
                    AlertInfo = $"Registration for {site.Name} has not opened yet";
                }
                return RedirectToAction("Index", "Home");
            }
            else if (siteStage >= SiteStage.ProgramEnded)
            {
                AlertInfo = $"{site.Name} has ended, please join us next time!";
                return RedirectToAction("Index", "Home");
            }

            PageTitle = $"{site.Name} - Join Now!";

            var branchList = await _siteService.GetBranches(1);

            JoinViewModel viewModel = new JoinViewModel()
            {
                BranchList = new SelectList(branchList.ToList(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(JoinViewModel model, string sitePath = null)
        {
            var site = await GetCurrentSiteAsync(sitePath);

            if (ModelState.IsValid)
            {
                model.ProgramId = 1;
                model.SiteId = site.Id;
                model.SystemId = 1;

                User user = _mapper.Map<User>(model);
                try
                {
                    await _userService.RegisterUserAsync(user, model.Password);
                    await LoginUserAsync(await _authenticationService
                        .AuthenticateUserAsync(user.Username, model.Password));

                    return RedirectToAction("Index", "Home");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(gex.Message);
                }
            }

            PageTitle = $"{site.Name} - Join Now!";

            var branchList = await _siteService.GetBranches(1);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");

            return View(model);
        }
    }
}
