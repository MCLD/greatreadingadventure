using System;
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
    }
}
