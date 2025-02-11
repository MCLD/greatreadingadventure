using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Programs
{
    public class ProgramDetailViewModel
    {
        public string Action { get; set; }

        [Range(0, 2)]
        public int AgeValues { get; set; }

        [DisplayName("Badge Alternative Text")]
        [MaxLength(255)]
        public string BadgeAltText { get; set; }

        public string BadgeMakerImage { get; set; }
        public string BadgeMakerUrl { get; set; }
        public string BadgePath { get; set; }

        [DisplayName("Upload a badge image. Badge images must be square.")]
        public IFormFile BadgeUploadImage { get; set; }

        public SelectList DailyLiteracyTipList { get; set; }
        public SelectList PointTranslationList { get; set; }
        public Program Program { get; set; }

        [Range(0, 2)]
        public int SchoolValues { get; set; }

        public bool UseBadgeMaker { get; set; }
    }
}
