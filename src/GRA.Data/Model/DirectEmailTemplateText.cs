using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DirectEmailTemplateText
    {
        [Required]
        public string BodyCommonMark { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public DirectEmailTemplate DirectEmailTemplate { get; set; }

        [Key]
        public int DirectEmailTemplateId { get; set; }

        [Required]
        public string Footer { get; set; }

        public Language Language { get; set; }

        [Key]
        public int LanguageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Preview { get; set; }

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
