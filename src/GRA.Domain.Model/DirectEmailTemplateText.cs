using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DirectEmailTemplateText
    {
        [Required]
        public string BodyCommonMark { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Key]
        public int DirectEmailTemplateId { get; set; }

        [Required]
        public string Footer { get; set; }

        public string ImportCulture { get; set; }
        public string ImportSystemEmailId { get; set; }

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
