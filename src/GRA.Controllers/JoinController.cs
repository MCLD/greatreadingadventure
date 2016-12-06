using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.ViewModel.Join;
using Microsoft.AspNetCore.Mvc.Rendering;
using GRA.Domain.Service;
using GRA.Domain.Model;
using System;

namespace GRA.Controllers
{
    public class JoinController : Base.Controller
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
            var site = await GetCurrentSite(sitePath);
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
            var site = await GetCurrentSite(sitePath);

            if (ModelState.IsValid)
            {
                model.ProgramId = 1;
                model.SiteId = site.Id;
                model.SystemId = 1;

                User user = _mapper.Map<User>(model);
                try
                {
                    await _userService.RegisterUserAsync(user, model.Password);
                } catch (Exception ex)
                {
                    AlertDanger = ex.Message;
                    return View(model);
                }
                await LoginUserAsync(await _authenticationService
                    .AuthenticateUserAsync(user.Username, model.Password));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                PageTitle = $"{site.Name} - Join Now!";

                var branchList = await _siteService.GetBranches(1);
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");

                return View(model);
            }
        }
    }
}
