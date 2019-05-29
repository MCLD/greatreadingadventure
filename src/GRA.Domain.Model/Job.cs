using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Job : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public JobType JobType { get; set; }

        public string SerializedParameters { get; set; }

        [Required]
        public Guid JobToken { get; set; }
    }
}
