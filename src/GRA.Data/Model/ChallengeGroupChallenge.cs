using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeGroupChallenge
    {
        public int ChallengeGroupId { get; set; }
        public virtual ChallengeGroup ChallengeGroup { get; set; }
        public int ChallengeId { get; set; }
        public virtual Challenge Challenge { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
