using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AtAGlanceReport
    {
        public StatusSummary SiteStatus { get; set; }
        public string FilteredBranchDescription { get; set; }
        public StatusSummary FilteredStatus { get; set; }
        public int LatestNewsId { get; set; }
    }
}
