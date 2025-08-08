using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipUploadViewModel
    {

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
