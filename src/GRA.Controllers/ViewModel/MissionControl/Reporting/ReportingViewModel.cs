using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportingViewModel
    {
        public GRA.Domain.Model.StatusSummary Request { get; set; }
        public IEnumerable<GRA.Domain.Model.StatusSummary> StatusSummaries { get; set; }
        public int TotalUsers { get; set; }
        public long TotalBooksRead { get; set; }
        public long TotalChallengesCompleted { get; set; }
        public long TotalPointsEarned { get; set; }
        public long TotalBadgesEarned { get; set; }
    }
}
