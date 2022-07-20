using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class FeaturedChallengeGroup : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }

        public int ChallengeGroupId { get; set; }
        public ChallengeGroup ChallengeGroup { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int SortOrder { get; set; }
    }
}
