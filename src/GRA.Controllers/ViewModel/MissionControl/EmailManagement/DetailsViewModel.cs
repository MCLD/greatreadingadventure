using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.EmailManagement
{
    public class DetailsViewModel
    {
        public string Action { get; set; }

        [Required]
        [Display(Name = "Email body", Description = "Specified in CommonMark")]
        public string BodyCommonMark { get; set; }

        public string DefaultTestEmail { get; set; }

        [Required]
        [Display(Name = "Email base template")]
        public int EmailBaseId { get; set; }

        public SelectList EmailBases { get; set; }
        public int EmailTemplateId { get; set; }

        [Display(Name = "Email footer", Description = "Specified in CommonMark")]
        [Required]
        public string Footer { get; set; }

        public bool IsDisabled { get; set; }

        [Required]
        [Display(Name = "Language")]
        public int LanguageId { get; set; }

        public SelectList Languages { get; set; }

        [Required]
        [Display(Name = "Email preview",
            Description = "Shown to recipient before they open the email, up to 255 characters")]
        [MaxLength(255)]
        public string Preview { get; set; }

        [DisplayName("Email addresses for test email, comma separated")]
        public string SendTestRecipients { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Email subject", Description = "Up to 255 characters")]
        public string Subject { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Template name")]
        public string TemplateDescription { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Email title", Description = "Up to 255 characters")]
        public string Title { get; set; }

        [Display(Name = "Import template .json")]
        public IFormFile UploadedFile { get; set; }
    }
}
