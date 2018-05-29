using System;
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

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }
        public string GroupName { get; set; }
        public string SchoolDistrictName { get; set; }
        public string SchoolName { get; set; }
        public string VendorCodeName { get; set; }

        public int ReportResultId { get; set; }
    }
}
