﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Challenges
{
    public class ChallengeDetailViewModel
    {
        public GRA.Domain.Model.Challenge Challenge { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool ShowCompleted { get; set; }
        public string TaskCountAndDescription { get; set; }
        public string PointCountAndDescription { get; set; }
        public bool IsBadgeEarning { get; set; }
        public string BadgePath { get; set; }

        [MaxLength(255)]
        public string BadgeAltText { get; set; }
        public List<TaskDetailViewModel> Tasks { get; set; }
    }
}
