using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GRA.Domain.Model
{
    public class DashboardContent : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        public string Content { get; set; }
        [Required]
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
    }
}
