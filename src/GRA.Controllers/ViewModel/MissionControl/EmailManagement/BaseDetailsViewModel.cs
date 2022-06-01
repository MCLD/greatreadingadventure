using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class BaseDetailsViewModel
    {
        public string Action { get; set; }

        public int EmailBaseId { get; set; }

        [Required]
        [Display(Name = "Language")]
        public int LanguageId { get; set; }

        public SelectList Languages { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "HTML Email Template")]
        [Required]
        public string TemplateHtml { get; set; }

        [Required]
        [Display(Name = "MJML",
            Description = "Not used by this software, for reference purposes only")]
        public string TemplateMjml { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Text Email Template")]
        public string TemplateText { get; set; }

        public IFormFile UploadedFile { get; set; }
    }
}
