using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Reporting
{
    public class ReportResultsViewModel
    {
        public string BranchName { get; set; }

        public int Column { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<string> FirstRow { get; set; }

        public string GroupName { get; set; }

        public IEnumerable<string> LastRow { get; set; }

        public string Message { get; set; }

        public string ProgramName { get; set; }

        public int ReportResultId { get; set; }

        public StoredReportSet ReportSet { get; set; }

        public ReportCriterion Request { get; set; }

        public IEnumerable<IEnumerable<string>> Results { get; set; }

        public int Row { get; set; }

        public string SchoolDistrictName { get; set; }

        public string SchoolName { get; set; }

        public DateTime? StartDate { get; set; }

        public string SystemName { get; set; }

        public string Title { get; set; }

        public string VendorCodeName { get; set; }

        public string CurrentRecordLink(StoredReport report)
        {
            return report != null
                && report.Links?.Count > 0
                && report.Links.TryGetValue($"{Row},{Column}", out string link)
                ? link
                : null;
        }
    }
}
