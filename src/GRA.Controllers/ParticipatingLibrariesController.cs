using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.ParticipatingBranches;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers
{
    public class ParticipatingLibrariesController : Base.UserController
    {
        private readonly SiteService _siteService;

        public ParticipatingLibrariesController(ServiceFacade.Controller context,
            SiteService siteService)
            : base(context)
        {
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public static string Name { get { return "ParticipatingLibraries"; } }

        public async Task<IActionResult> Index()
        {
            return View(nameof(Index), new ParticipatingLibrariesViewModel
            {
                Systems = await _siteService.GetSystemList()
            });
        }
    }
}
