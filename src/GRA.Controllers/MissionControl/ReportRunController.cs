using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Reporting;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportRunController : Base.MCController
    {
        private readonly ReportRunService _reportRunService;

        public ReportRunController(ServiceFacade.Controller context,
            ReportRunService reportRunService)
            : base(context)
        {
            _reportRunService = reportRunService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int take = 20,
            int? reportId = null, int? requestedBy = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            if (page < 1) { page = 1; }
            if (take < 5) { take = 5; }
            if (take > 100) { take = 100; }

            var skip = (page - 1) * take;

            var filter = new ReportRequestFilter
            {
                Skip = skip,
                Take = take,
                ReportId = reportId,
                RequestedByUserId = requestedBy,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _reportRunService.GetPaginatedReportRunsAsync(filter);

            var vm = new ReportRunIndexViewModel
            {
                Runs = result.Data,
                Pagination = new PaginateViewModel
                {
                    ItemCount = result.Count,
                    ItemsPerPage = take,
                    CurrentPage = page
                },
                Filter = filter
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult View(int id)
            => RedirectToAction("View", "Reporting", new { area = "MissionControl", id });

        [HttpGet]
        public IActionResult Download(int id)
            => RedirectToAction("Download", "Reporting", new { area = "MissionControl", id });
    }
}

