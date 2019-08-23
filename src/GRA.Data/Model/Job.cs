using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Job : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public Domain.Model.JobType JobType { get; set; }

        public string SerializedParameters { get; set; }

        [Required]
        public Guid JobToken { get; set; }

        public DateTime? Started { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Cancelled { get; set; }
        public DateTime? StatusAsOf { get; set; }

        [MaxLength(255)]
        public string Status { get; set; }
    }
}
