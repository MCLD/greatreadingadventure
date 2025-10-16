using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportHistoryViewModel : PaginateViewModel
    {
        public ReportHistoryViewModel()
        {
            UserNames = new Dictionary<int, string>();
            Requests = new List<ReportRequestSummary>();
        }

        public string ButtonAllClass
        {
            get
            {
                return ViewSelf ? "btn-outline-secondary" : "btn-primary";
            }
        }

        public string ButtonSelfClass
        {
            get
            {
                return ViewSelf ? "btn-primary" : "btn-outline-secondary";
            }
        }

        public int CurrentUser { get; set; }
        public ReportRequestFilter Filter { get; set; }
        public ICollection<ReportRequestSummary> Requests { get; }
        public IDictionary<int, string> UserNames { get; }
        public bool ViewAll { get; set; }
        public bool ViewAllPermissions { get; set; }
        public bool ViewSelf { get; set; }
    }
}
