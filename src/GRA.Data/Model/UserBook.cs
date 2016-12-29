using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserBook
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        int? ChallengeId { get; set; }
    }
}
