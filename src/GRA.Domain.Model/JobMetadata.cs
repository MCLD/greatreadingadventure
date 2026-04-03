using System;
using System.Threading;

namespace GRA.Domain.Model
{
    public class JobMetadata
    {
        public CancellationToken CancellationToken { get; set; }
        public int JobId { get; set; }
        public IProgress<JobStatus> Progress { get; set; }
        public Site Site { get; set; }
        public User UserContactDetails { get; set; }
        public string UserExportDetails
        {
            get
            {
                return $"{UserContactDetails?.FullName} ({UserContactDetails?.Email ?? UserContactDetails?.Username})";
            }
        }
    }
}
