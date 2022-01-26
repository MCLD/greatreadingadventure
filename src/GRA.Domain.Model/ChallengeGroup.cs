using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ChallengeGroup : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        [Required]
        public string Description { get; set; }
        [MaxLength(255)]
        [Required]
        public string Stub { get; set; }

        public IEnumerable<int> ChallengeIds { get; set; }
        public ICollection<Challenge> Challenges { get; set; }
        public DateTime? FeatureStartDate { get; set; }
        public DateTime? FeatureEndDate { get; set; }

        [MaxLength(255)]
        [Display(Name = "Image Path",
            Description = "The image you will see in the featured challenge group's carousel.")]
        public string ImagePath { get; set; }

        [MaxLength(255)]
        [DisplayName("Alternative Text")]
        public string AltText { get; set; }
    }
}
