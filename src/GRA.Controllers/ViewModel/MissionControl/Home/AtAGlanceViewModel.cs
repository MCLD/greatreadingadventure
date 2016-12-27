using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Home
{
    public class AtAGlanceViewModel
    {
        public StatusSummary SiteStatus { get; set; }
        public string FilteredBranchDescription { get; set; }
        public StatusSummary FilteredStatus { get; set; }
    }
}
