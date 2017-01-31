using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Event : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTime StartsAt { get; set; }
        [Required]
        public DateTime EndsAt { get; set; }
        [Required]
        public bool AllDay { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsValid { get; set; }

        public string ExternalLink { get; set; }

        public int? AtBranchId { get; set; }
        public int? AtLocationId { get; set; }

        public int? ProgramId { get; set; }

        public string EventLocationName { get; set; }
        public string EventLocationAddress { get; set; }
        public string EventLocationLink { get; set; }
        public string EventLocationTelephone { get; set; }
    }
}
