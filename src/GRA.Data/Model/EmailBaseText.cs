using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class EmailBaseText
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        public EmailBase EmailBase { get; set; }

        [Key]
        public int EmailBaseId { get; set; }

        public Language Language { get; set; }

        [Key]
        public int LanguageId { get; set; }

        public string TemplateHtml { get; set; }
        public string TemplateMjml { get; set; }
        public string TemplateText { get; set; }
    }
}
