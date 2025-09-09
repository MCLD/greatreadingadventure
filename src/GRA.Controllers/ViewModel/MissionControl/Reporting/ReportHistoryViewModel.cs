using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportHistoryViewModel
    {
        public ICollection<ReportRequestSummary> Requests { get; set; }
        public PaginateViewModel Pagination { get; set; }
        public ReportRequestFilter Filter { get; set; }
    }
}
