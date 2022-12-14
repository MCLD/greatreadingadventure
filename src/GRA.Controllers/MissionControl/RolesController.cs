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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageRoles)]
    public class RolesController : Base.MCController
    {
        private readonly ILogger<RolesController> _logger;
        private readonly AuthorizationCodeService _authorizationCodeService;
        private readonly RoleService _roleService;
        private readonly UserService _userService;
        public static string Name { get { return "Roles"; } }

        public RolesController(ILogger<RolesController> logger,
            ServiceFacade.Controller context,
            AuthorizationCodeService authorizationCodeService,
            RoleService roleService,
            UserService userService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authorizationCodeService = authorizationCodeService
                ?? throw new ArgumentNullException(nameof(authorizationCodeService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        #region Roles

        public async Task<IActionResult> Index(int page)
        {
            var filter = new BaseFilter(page == default ? 1 : page);

            var roleList = await _roleService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = roleList.Count,
                CurrentPage = page == default ? 1 : page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = paginateModel.LastPage ?? 1 });
            }

            var viewModel = new RoleListViewModel
            {
                Roles = roleList.Data,
                PaginateModel = paginateModel,
            };

            viewModel.SetUsersInRoles(await _roleService
                .GetUserCountForRolesAsync(roleList.Data.Select(_ => _.Id)));

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

            var viewModel = new RoleDetailViewModel
            {
                Action = nameof(Create),
                UnselectedPermissions = await _roleService.GetAllPermissionsAsync()
            };

            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleDetailViewModel model)
        {
            model.Role.Name = model.Role.Name?.Trim();

            if (ModelState.IsValid)
            {
                var permissionList = model.Permissions?.Split(',') ?? Array.Empty<string>();
                var role = await _roleService.AddAsync(model.Role, permissionList);
                ShowAlertSuccess($"Added Role \"{role.Name}\"!");
                return RedirectToAction(nameof(Edit), new { id = role.Id });
            }

            var unselected = await _roleService.GetAllPermissionsAsync();

            model.UnselectedPermissions = model.SelectedPermissions != null
                ? unselected.Except(model.SelectedPermissions)
                : unselected;

            PageTitle = "Create Role";
            return View("Detail", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Role";

            var viewModel = new RoleDetailViewModel
            {
                Role = await _roleService.GetByIdAsync(id),
                Action = nameof(Edit),
                SelectedPermissions = await _roleService.GetPermissionsForRoleAsync(id)
            };

            viewModel.UnselectedPermissions = (await _roleService.GetAllPermissionsAsync())
                .Except(viewModel.SelectedPermissions);

            if (viewModel.Role.IsAdmin)
            {
                ShowAlertWarning("Permissions for the System Administrator role cannot be modified. This role always has all permissions.");
            }

            var usersInRoles = await _roleService.GetUserCountForRolesAsync(new[] { id });

            viewModel.UsersInRole = usersInRoles?.ContainsKey(id) == true ? usersInRoles[id] : 0;

            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleDetailViewModel model)
        {
            model.SelectedPermissions = model.Permissions?.Split(',') ?? Array.Empty<string>();
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

        [HttpGet]
        public async Task<IActionResult> UsersInRole(int id, int page)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                ShowAlertDanger("Unknown role id {id}.");
                return RedirectToAction(nameof(Index));
            }

            var filter = new BaseFilter(page == default ? 1 : page)
            {
                SiteId = GetCurrentSiteId()
            };

            var users = await _userService.GetUserInfoByRole(id, filter);

            var viewModel = new UsersInRoleViewModel
            {
                CurrentPage = page == default ? 1 : page,
                ItemCount = users.Count,
                ItemsPerPage = filter.Take.Value,
                RoleId = id,
                RoleName = role.Name
            };

            viewModel.AddUsers(users.Data);

            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = viewModel.LastPage ?? 1 }); 
            }

            return View(viewModel);
        }

        #endregion Roles

        #region Authorization Codes

        public async Task<IActionResult> AuthorizationCodes(int page = 1)
        {
            var filter = new BaseFilter(page);

            var authorizationCodeList = await _authorizationCodeService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = authorizationCodeList.Count,
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

            var viewModel = new AuthorizationCodeListViewModel
            {
                AuthorizationCodes = authorizationCodeList.Data,
                PaginateModel = paginateModel,
                RoleList = new SelectList(await _roleService.GetAllAsync(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCode(AuthorizationCodeListViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Unable to add empty code.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                model.AuthorizationCode.Code = model.AuthorizationCode.Code.Trim();
                model.AuthorizationCode.Description = model.AuthorizationCode.Description?.Trim();
                await _authorizationCodeService.AddAsync(model.AuthorizationCode);
                ShowAlertSuccess($"Added Authorization Code \"{model.AuthorizationCode.Code}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Authorization Code: ", gex);
            }
            return RedirectToAction(nameof(AuthorizationCodes), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditCode(AuthorizationCodeListViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Unable to edit empty code.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                model.AuthorizationCode.Code = model.AuthorizationCode.Code.Trim();
                model.AuthorizationCode.Description = model.AuthorizationCode.Description?.Trim();
                await _authorizationCodeService.UpdateAsync(model.AuthorizationCode);
                ShowAlertSuccess($"Authorization Code \"{model.AuthorizationCode.Code}\" updated!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit Authorization Code: ", gex);
            }

            return RedirectToAction(nameof(AuthorizationCodes), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCode(AuthorizationCodeListViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Unable to delete empty code.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _authorizationCodeService.RemoveAsync(model.AuthorizationCode.Id);
                ShowAlertSuccess($"Authorization Code \"{model.AuthorizationCode.Code}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Authorization Code: ", gex);
            }

            return RedirectToAction(nameof(AuthorizationCodes), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        #endregion Authorization Codes
    }
}
