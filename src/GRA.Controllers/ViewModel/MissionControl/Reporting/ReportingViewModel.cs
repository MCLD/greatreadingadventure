using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportingViewModel
    {
        public GRA.Domain.Model.StatusSummary Request { get; set; }
        public IEnumerable<GRA.Domain.Model.StatusSummary> StatusSummaries { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBooksRead { get; set; }
        public int TotalChallengesCompleted { get; set; }
        public int TotalPointsEarned { get; set; }
    }
}
