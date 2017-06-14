using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportResultsViewModel
    {
        public Domain.Model.ReportCriterion Request { get; set; }
        public Domain.Model.StoredReportSet ReportSet { get; set; }
        public string Title { get; set; }
        public IEnumerable<IEnumerable<string>> Results { get; set; }
        public IEnumerable<string> FirstRow { get; set; }
        public IEnumerable<string> LastRow { get; set; }

        public int ReportResultId { get; set; }
    }
}
