using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Broadcast : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }
        public DateTime SendAt { get; set; }
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Body { get; set; }
        public bool SendToNewUsers { get; set; }
    }
}
