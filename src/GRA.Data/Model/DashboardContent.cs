using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DashboardContent : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        public string Content { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
    }
}
