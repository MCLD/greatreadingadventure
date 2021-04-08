using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipUploadViewModel
    {
        [Required]
        [Display(Name = "Are the images wider than 600 pixels?",
            Description = "Images will show in a larger pop-up if you choose yes")]
        public bool IsLarge { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Button to show on dashboard",
            Description = "This is the button that participants will click to see the pop-up image")]
        public string Message { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Administrative name of these buttons",
            Description = "This is only shown in Mission Control")]
        public string Name { get; set; }

        [Required]
        public IFormFile UploadedFile { get; set; }
    }
}