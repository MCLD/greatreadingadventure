using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public IDictionary<string, int> Languages { get; set; }
        public IDictionary<int, int[]> SegmentLanguageIds { get; set; }

        public string LanguageSegmentClass(int? segmentId, int languageId)
        {
            return !segmentId.HasValue
                ? "btn-secondary"
                : SegmentLanguageIds.TryGetValue(segmentId.Value, out int[] value)
                    && value.Contains(languageId)
                    ? "btn-success"
                    : "btn-warning";
        }
    }
}
