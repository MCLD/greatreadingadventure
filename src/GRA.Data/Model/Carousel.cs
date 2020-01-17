using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Carousel : Abstract.BaseDbEntity
    {
        [MaxLength(255)]
        public string Heading { get; set; }

        [Required]
        public bool IsForDashboard { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
    }
}
