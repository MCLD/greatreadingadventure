using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
