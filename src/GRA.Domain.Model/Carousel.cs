using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Carousel : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        [Display(Description = "optional heading to display above the carousel")]
        public string Heading { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public bool IsForDashboard { get; set; }

        public IEnumerable<CarouselItem> Items { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Description = "for Mission Control use only")]
        public string Name { get; set; }

        public int SiteId { get; set; }

        [Required]
        [Display(Name = "Start time",
            Description = "begin displaying this carousel at this time and date")]
        public DateTime StartTime { get; set; }

        public int ItemCount { get; set; }
    }
}
