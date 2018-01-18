using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Programs
{
    public class ProgramDetailViewModel
    {
        public Program Program { get; set; }
        public string Action { get; set; }

        [Range(0, 2)]
        public int AgeValues { get; set; }
        [Range(0, 2)]
        public int SchoolValues { get; set; }

        public string BadgePath { get; set; }
        public IFormFile BadgeUploadImage { get; set; }
        public string BadgeMakerUrl { get; set; }
        public bool UseBadgeMaker { get; set; }
        public string BadgeMakerImage { get; set; }

        public SelectList DailyLiteracyTipList { get; set; }
        public SelectList PointTranslationList { get; set; }
    }
}
