using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class TriggerBadge
    {
        [Required]
        public int TriggerId { get; set; }
        public Trigger Trigger { get; set; }
        [Required]
        public int BadgeId { get; set; }
        public Badge Badge { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
