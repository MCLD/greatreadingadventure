using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.GroupTypes;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageGroupTypes)]
    public class GroupTypesController : Base.MCController
    {
        private const int PaginationTake = 15;

        private readonly GroupTypeService _groupTypesService;

        public GroupTypesController(ServiceFacade.Controller context,
            GroupTypeService groupTypesService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(groupTypesService);

            _groupTypesService = groupTypesService;

            PageTitle = "Group Type management";
        }

        public static string Name
        { get { return "GroupTypes"; } }

        public async Task<IActionResult> Add(GroupTypesListViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (viewModel.GroupType == null
               || string.IsNullOrEmpty(viewModel.GroupType.Name)
               || string.IsNullOrEmpty(viewModel.GroupType.Name.Trim()))
            {
                AlertWarning = "Unable to add group type - must supply a name.";
            }
            else
            {
                var (result, message)
                    = await _groupTypesService.Add(GetActiveUserId(), viewModel.GroupType.Name);
                if (result)
                {
                    AlertSuccess = $"Successfully added group type <strong>{message}</strong>.";
                }
                else
                {
                    AlertDanger = $"Could not add group type: {message}";
                }
            }
            return RedirectToAction("Index",
                new { page = viewModel?.PaginateModel?.CurrentPage ?? 1 });
        }

        public async Task<IActionResult> Delete(GroupTypesListViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (viewModel.GroupType == null)
            {
                AlertWarning = "Unable to remove group type - must supply an id.";
            }
            else
            {
                string result
                    = await _groupTypesService.Remove(GetActiveUserId(), viewModel.GroupType.Id);
                if (string.IsNullOrEmpty(result))
                {
                    AlertSuccess = $"Successfully removed group type <strong>{viewModel.GroupType.Name}</strong>.";
                }
                else
                {
                    AlertDanger = $"Could not remove group type {viewModel.GroupType.Name}: {result}";
                }
            }

            return RedirectToAction("Index",
                new { page = viewModel?.PaginateModel?.CurrentPage ?? 1 });
        }

        public async Task<IActionResult> Edit(GroupTypesListViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (viewModel.GroupType == null
               || string.IsNullOrEmpty(viewModel.GroupType.Name)
               || string.IsNullOrEmpty(viewModel.GroupType.Name.Trim()))
            {
                AlertWarning = "Unable to modify group type - must supply a name.";
            }
            else
            {
                var (result, message) = await _groupTypesService.Edit(GetActiveUserId(),
                    viewModel.GroupType.Id,
                    viewModel.GroupType.Name);

                if (result)
                {
                    AlertSuccess = $"Successfully modified group type <strong>{message}</strong>.";
                }
                else
                {
                    AlertDanger = $"Could not modify group type: {message}";
                }
            }
            return RedirectToAction("Index",
                new { page = viewModel?.PaginateModel?.CurrentPage ?? 1 });
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int skip = PaginationTake * (page - 1);
            var data = await _groupTypesService.GetAllMCPagedAsync(skip, PaginationTake);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = data.Count,
                CurrentPage = page,
                ItemsPerPage = PaginationTake
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var (useGroups, maximumHousehold) =
                await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);

            return View(new GroupTypesListViewModel
            {
                SiteId = GetCurrentSiteId(),
                GroupTypes = data.Data,
                PaginateModel = paginateModel,
                MaximumHouseholdMembers = useGroups ? (int?)maximumHousehold : null
            });
        }
    }
}
