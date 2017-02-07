using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserTrigger
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int TriggerId { get; set; }
        public Trigger Trigger { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
