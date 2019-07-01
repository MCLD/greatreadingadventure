using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class NewsCategory : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public ICollection<NewsPost> Posts { get; set; }

        public DateTime? LastPostDate { get; set; }

    }
}
