using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class SocialHeader : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
    }
}