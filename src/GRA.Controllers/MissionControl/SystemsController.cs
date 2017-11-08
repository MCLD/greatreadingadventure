using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Systems;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSystems)]
    public class SystemsController : Base.MCController
    {
        private readonly ILogger<SystemsController> _logger;
        private readonly SiteService _siteService;
        public SystemsController(ILogger<SystemsController> logger,
            ServiceFacade.Controller context,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "System";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var systemList = await _siteService.GetPaginatedSystemListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = systemList.Count,
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

            SystemListViewModel viewModel = new SystemListViewModel()
            {
                Systems = systemList.Data.ToList(),
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSystem(SystemListViewModel model)
        {
            try
            {
                await _siteService.AddSystemAsync(model.System);
                ShowAlertSuccess($"Added System '{model.System.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add System: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditSystem(SystemListViewModel model)
        {
            try
            {
                await _siteService.UpdateSystemAsync(model.System);
                ShowAlertSuccess($"System  '{model.System.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit System: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int id, string search)
        {
            try
            {
                await _siteService.RemoveSystemAsync(id);
                AlertSuccess = "System removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete System: ", gex);
            }
            return RedirectToAction("Index", new { search = search });
        }

        public async Task<IActionResult> Branches(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var branchList = await _siteService.GetPaginatedBranchListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = branchList.Count,
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

            BranchesListViewModel viewModel = new BranchesListViewModel()
            {
                Branches = branchList.Data.ToList(),
                PaginateModel = paginateModel,
                SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBranch(BranchesListViewModel model)
        {
            try
            {
                await _siteService.AddBranchAsync(model.Branch);
                ShowAlertSuccess($"Added Branch '{model.Branch.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Branch: ", gex);
            }
            return RedirectToAction("Branches", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditBranch(BranchesListViewModel model)
        {
            try
            {
                await _siteService.UpdateBranchAsync(model.Branch);
                ShowAlertSuccess($"Branch  '{model.Branch.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit Branch: ", gex);
            }
            return RedirectToAction("Branches", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(int id, string search)
        {
            try
            {
                await _siteService.RemoveBranchAsync(id);
                AlertSuccess = "Branch removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete Branch: ", gex);
            }
            return RedirectToAction("Branches", new { search = search });
        }
    }
}
