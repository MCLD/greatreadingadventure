using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
    }
}
