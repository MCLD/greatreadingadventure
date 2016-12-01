using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ServiceFacade;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using GRA.Controllers.ViewModel.Join;
using Microsoft.AspNetCore.Mvc.Rendering;
using GRA.Domain.Service;
using GRA.Domain.Model;
using AutoMapper;

namespace GRA.Controllers
{
    public class JoinController : Base.Controller
    {
        private readonly ILogger<JoinController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly UserService _userService;
        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context,
            UserService userService)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Join";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSite(sitePath);
            PageTitle = $"{site.Name} - Join Now!";

            var branchList = await _siteService.GetBranches(CurrentUser, site.Id);

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

                await _userService.RegisterUserAsync(user, model.Password);
                await LoginUserAsync(await _userService
                    .AuthenticateUserAsync(user.Username, model.Password));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                PageTitle = $"{site.Name} - Join Now!";

                var branchList = await _siteService.GetBranches(CurrentUser, site.Id);
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");

                return View(model);
            }
        }
    }
}
