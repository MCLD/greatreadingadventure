using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class JobDetailsRunReport
    {
        public JobDetailsRunReport()
        {
            Properties = new Dictionary<JobDetailsPropertyName, string>();
        }

        public string CancelUrl { get; set; }
        public IDictionary<JobDetailsPropertyName, string> Properties { get; set; }
        public int ReportRequestId { get; set; }
        public string SuccessUrl { get; set; }
    }
}
