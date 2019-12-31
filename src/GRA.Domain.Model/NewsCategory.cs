using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class NewsCategory : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public int PostCount { get; set; }

        public DateTime LastPostDate { get; set; }
        public bool IsNew { get; set; }

    }
}
