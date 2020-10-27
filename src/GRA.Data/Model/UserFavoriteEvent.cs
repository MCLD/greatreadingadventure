using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserFavoriteEvent
    {
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        public int EventId { get; set; }
        public virtual Event Event { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreatedBy { get; set; }
    }
}
