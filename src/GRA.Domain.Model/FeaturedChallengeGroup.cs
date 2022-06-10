using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class FeaturedChallengeGroup : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        public int SortOrder { get; set; }

        [DisplayName("Challenge Group")]
        public int ChallengeGroupId { get; set; }
        public ChallengeGroup ChallengeGroup { get; set; }

        public FeaturedChallengeGroupText FeaturedGroupText { get; set; }

        public bool IsActive(DateTime now)
        {
            return (!StartDate.HasValue || StartDate.Value <= now)
                && (!EndDate.HasValue || EndDate.Value >= now);
        }
    }
}
