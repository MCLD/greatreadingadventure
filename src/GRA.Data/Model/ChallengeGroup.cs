using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeGroup : Abstract.BaseDbEntity
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

        public virtual ICollection<ChallengeGroupChallenge> ChallengeGroupChallenges { get; set; }
    }
}
