using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportRunIndexViewModel
    {
        public ICollection<ReportRunSummary> Runs { get; set; }
        public PaginateViewModel Pagination { get; set; }
        public ReportRequestFilter Filter { get; set; }
    }
}
