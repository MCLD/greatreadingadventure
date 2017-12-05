using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserFavoriteChallenge
    {
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public int ChallengeId { get; set; }
        public virtual Challenge Challenge { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
    }
}
