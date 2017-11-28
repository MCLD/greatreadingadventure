using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserFavoriteChallenge
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}
