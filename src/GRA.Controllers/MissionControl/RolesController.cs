using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Roles;
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
    [Authorize(Policy = Policy.ManageRoles)]
    public class RolesController : Base.MCController
    {
        private readonly ILogger<RolesController> _logger;
        private readonly RoleService _roleService;
        public RolesController(ILogger<RolesController> logger,
            ServiceFacade.Controller context,
            RoleService roleService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);

            var roleList = await _roleService.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = roleList.Count,
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

            var viewModel = new RoleListViewModel()
            {
                Roles = roleList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RoleListViewModel model)
        {
            try
            {
                await _roleService.RemoveAsync(model.Role.Id);
                ShowAlertSuccess($"Role \"{model.Role.Name}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Role: ", gex);
            }

            return RedirectToAction(nameof(Index), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> Create()
        {
            PageTitle = "Create Role";

            var viewModel = new RoleDetailViewModel()
            {
                Action = nameof(Create),
                UnselectedPermissions = await _roleService.GetAllPermissionsAsync()
            };

            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var permissionList = model.Permissions?.Split(',') ?? new string[] { };
                var role = await _roleService.AddAsync(model.Role, permissionList);
                ShowAlertSuccess($"Added Role \"{role.Name}\"!");
                return RedirectToAction(nameof(Edit), new { id = role.Id });
            }

            model.UnselectedPermissions = (await _roleService.GetAllPermissionsAsync())
                .Except(model.SelectedPermissions);

            PageTitle = "Create Role";
            return View("Detail", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Role";

            var viewModel = new RoleDetailViewModel()
            {
                Role = await _roleService.GetByIdAsync(id),
                Action = nameof(Edit),
                SelectedPermissions = await _roleService.GetPermissionsForRoleAsync(id)
            };

            viewModel.UnselectedPermissions = (await _roleService.GetAllPermissionsAsync())
                .Except(viewModel.SelectedPermissions);

            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleDetailViewModel model)
        {
            model.SelectedPermissions = model.Permissions?.Split(',') ?? new string[] { };
            if (ModelState.IsValid)
            {
                await _roleService.EditAsync(model.Role, model.SelectedPermissions);
                ShowAlertSuccess($"Saved Role \"{model.Role.Name}\"!");
                return RedirectToAction(nameof(Edit), new { id = model.Role.Id });
            }

            model.UnselectedPermissions = (await _roleService.GetAllPermissionsAsync())
                .Except(model.SelectedPermissions);

            PageTitle = "Edit Role";
            return View("Detail", model);
        }
    }
}
