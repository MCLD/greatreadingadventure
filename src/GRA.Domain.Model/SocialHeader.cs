using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class SocialHeader : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public DateTime? NextStartDate { get; set; }

        [Required]
        public int SiteId { get; set; }

        public ICollection<Social> Socials { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
    }
}