using System.ComponentModel.DataAnnotations;
using CsvHelper;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipImageAddViewModel
    {

        [Required]
        public int DailyTipId { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public string AllowedExtensions { get; set; }
    }
}
