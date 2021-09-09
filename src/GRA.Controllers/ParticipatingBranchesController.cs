using GRA.Domain.Service;
using GRA.Controllers.ViewModel.ParticipatingBranches;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class ParticipatingBranchesController : Base.UserController
    {
        private readonly SiteService _siteService;

        public static string Name { get { return "ParticipatingBranches"; } }

        public ParticipatingBranchesController(ServiceFacade.Controller context,
            SiteService siteService)
            : base(context)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ParticipatingBranchesViewModel
            {
                Systems = await _siteService.GetSystemList()
            };
            return View(nameof(Index), viewModel);
        }
    }
}
