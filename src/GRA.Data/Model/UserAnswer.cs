using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserAnswer
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
