using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Job : Abstract.BaseDomainEntity
    {
        public DateTime? Cancelled { get; set; }

        public DateTime? Completed { get; set; }

        public TimeSpan? Duration
        {
            get
            {
                return Completed.HasValue && Started.HasValue
                    ? Completed - Started
                    : null;
            }
        }

        [Required]
        public Guid JobToken { get; set; }

        [Required]
        public JobType JobType { get; set; }

        public string SerializedParameters { get; set; }

        [Required]
        public int SiteId { get; set; }

        public DateTime? Started { get; set; }

        [MaxLength(255)]
        public string Status { get; set; }

        public DateTime? StatusAsOf { get; set; }
    }
}
