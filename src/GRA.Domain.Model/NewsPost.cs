using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class NewsPost : Abstract.BaseDomainEntity
    {
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Required]
        public string Content { get; set; }

        public string CreatedByName { get; set; }

        [DisplayName("Brief summary of this post to include in emails")]
        [MaxLength(255)]
        [Required]
        public string EmailSummary { get; set; }

        [DisplayName("Pin post to the top?")]
        public bool IsPinned { get; set; }

        public int? NextPostId { get; set; }
        public int? PreviousPostId { get; set; }
        public DateTime? PublishedAt { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }
}
