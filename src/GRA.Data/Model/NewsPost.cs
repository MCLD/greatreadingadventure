using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class NewsPost : Abstract.BaseDbEntity
    {
        public int CategoryId { get; set; }
        public NewsCategory Category { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime? PublishedAt { get; set; }

        [MaxLength(255)]
        public string EmailSummary { get; set; }
    }
}
