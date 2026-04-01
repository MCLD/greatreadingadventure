using System;
using System.Threading;

namespace GRA.Domain.Model
{
    public class JobMetadata
    {
        public CancellationToken CancellationToken { get; set; }
        public int JobId { get; set; }
        public IProgress<JobStatus> Progress { get; set; }
    }
}
