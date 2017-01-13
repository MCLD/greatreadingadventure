using GRA.Controllers.ViewModel.MissionControl.Reporting;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllReporting)]
    public class ReportingController : Base.MCController
    {
        private readonly ILogger<ReportingController> _logger;
        private readonly ReportService _reportService;

        public ReportingController(ILogger<ReportingController> logger,
            ServiceFacade.Controller context,
            ReportService reportService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _reportService = Require.IsNotNull(reportService, nameof(reportService));
            PageTitle = "Reporting";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReportingViewModel model)
        {
            if (model.Request.StartDate.HasValue 
                && model.Request.EndDate.HasValue 
                && model.Request.EndDate <= model.Request.StartDate)
            {
                ModelState.AddModelError("Request.EndDate", "The End Date must be after the Start Date");
            }

            if (ModelState.IsValid)
            {
                var stats = await _reportService.GetAllByBranchAsync(model.Request);
                ReportingViewModel viewModel = new ReportingViewModel()
                {
                    Request = model.Request,
                    StatusSummaries = stats,
                    TotalUsers = stats.Sum(_ => _.RegisteredUsers),
                    TotalBooksRead = stats.Sum(_ => _.ActivityEarnings.Sum(m => m.Value)),
                    TotalChallengesCompleted = stats.Sum(_ => _.CompletedChallenges),
                    TotalBadgesEarned = stats.Sum(_ => _.BadgesEarned),
                    TotalPointsEarned = stats.Sum(_ => _.PointsEarned)
                };
                return View(viewModel);
            }
            else
            {
                return View(model);
            }
        }
    }
}
