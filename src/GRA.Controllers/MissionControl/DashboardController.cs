using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Dashboard;
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
    [Authorize(Policy = Policy.ManageDashboardContent)]
    public class DashboardController : Base.MCController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly DashboardContentService _dashboardContentService;
        public DashboardController(ILogger<DashboardController> logger,
            ServiceFacade.Controller context,
            DashboardContentService dashboardContentService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dashboardContentService = dashboardContentService
                ?? throw new ArgumentNullException(nameof(dashboardContentService));
            PageTitle = "Dashboard Content";
        }

        public async Task<IActionResult> Index(int page = 1, bool archived = false)
        {
            var filter = new BaseFilter(page)
            {
                IsActive = !archived
            };

            var dashboardContentList = await _dashboardContentService.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = dashboardContentList.Count,
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

            var viewModel = new DashboardListViewModel()
            {
                DashboardContents = dashboardContentList.Data.ToList(),
                PaginateModel = paginateModel
            };

            if (viewModel.DashboardContents.FirstOrDefault()?.StartTime < _dateTimeProvider.Now
                && archived == false)
            {
                viewModel.HighlightFirst = true;
            }

            return View(viewModel);
        }

        public IActionResult Create()
        {
            PageTitle = "Create Content";
            var viewModel = new DashboardDetailViewModel()
            {
                Action = "Create"
            };
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DashboardDetailViewModel model)
        {
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

        public async Task<IActionResult> Edit(int id)
        {
            PageTitle = "Edit Content";
            var viewModel = new DashboardDetailViewModel()
            {
                DashboardContent = await _dashboardContentService.GetByIdAsync(id),
                Action = "Edit"
            };
            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DashboardDetailViewModel model)
        {
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

        [HttpPost]
        public async Task<IActionResult> Delete(DashboardListViewModel model)
        {
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

        public async Task<IActionResult> Preview(int id)
        {
            PageTitle = $"Preview";
            var dashboardContent = await _dashboardContentService.GetByIdAsync(id);
            dashboardContent.Content = CommonMark.CommonMarkConverter.Convert(
                dashboardContent.Content);

            return View(dashboardContent);
        }
    }
}
