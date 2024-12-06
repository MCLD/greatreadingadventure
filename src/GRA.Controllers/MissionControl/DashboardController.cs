using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Dashboard;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageDashboardContent)]
    public class DashboardController : Base.MCController
    {
        private readonly DashboardContentService _dashboardContentService;

        public DashboardController(ServiceFacade.Controller context,
            DashboardContentService dashboardContentService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(dashboardContentService);

            _dashboardContentService = dashboardContentService;

            PageTitle = "Dashboard Content management";
        }

        public static string Name
        { get { return "Dashboard"; } }

        public IActionResult Create()
        {
            var viewModel = new DashboardDetailViewModel()
            {
                Action = "Create"
            };
            PageTitle = "Create Content";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DashboardDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var dashboardContent = await _dashboardContentService
                        .AddAsync(model.DashboardContent);
                    ShowAlertSuccess("Dashboard content was successfully added!");
                    return RedirectToAction("Edit", new { id = dashboardContent.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add content: ", gex);
                }
            }
            PageTitle = "Create Content";
            return View("Detail", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DashboardListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                await _dashboardContentService.RemoveAsync(model.DashboardContentId);
                ShowAlertSuccess("Dashboard content was successfully deleted!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete content: ", gex);
            }
            return RedirectToAction("Index", new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = new DashboardDetailViewModel()
            {
                DashboardContent = await _dashboardContentService.GetByIdAsync(id),
                Action = "Edit"
            };
            PageTitle = "Edit Content";
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DashboardDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            if (ModelState.IsValid)
            {
                try
                {
                    await _dashboardContentService.EditAsync(model.DashboardContent);
                    ShowAlertSuccess("Dashboard content was successfully edited!");
                    return RedirectToAction("Edit", new { id = model.DashboardContent.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit content: ", gex);
                }
            }
            PageTitle = "Edit Content";
            return View("Detail", model);
        }

        public async Task<IActionResult> Index(int page = 1, bool archived = false)
        {
            var filter = new BaseFilter(page)
            {
                IsActive = !archived
            };

            var dashboardContentList = await _dashboardContentService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = dashboardContentList.Count,
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

            var viewModel = new DashboardListViewModel()
            {
                DashboardContents = dashboardContentList.Data.ToList(),
                PaginateModel = paginateModel
            };

            if (viewModel.DashboardContents.FirstOrDefault()?.StartTime < _dateTimeProvider.Now
                && !archived)
            {
                viewModel.HighlightFirst = true;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Preview(int id)
        {
            var dashboardContent = await _dashboardContentService.GetByIdAsync(id);
            dashboardContent.Content = CommonMark.CommonMarkConverter.Convert(
                dashboardContent.Content);

            PageTitle = "Preview";
            return View(dashboardContent);
        }
    }
}
