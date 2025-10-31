using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class NewsPost : Abstract.BaseDbEntity
    {
        public NewsCategory Category { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(255)]
        public string EmailSummary { get; set; }

        public bool IsPinned { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
