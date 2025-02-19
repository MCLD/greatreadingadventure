using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengesDetailViewModel
    {
        public bool AddTask { get; set; }

        [DisplayName("Badge Alternative Text")]
        [MaxLength(255)]
        public string BadgeAltText { get; set; }

        public string BadgeMakerImage { get; set; }
        public string BadgePath { get; set; }

        [DisplayName("Upload a badge image. Badge images must be square.")]
        public IFormFile BadgeUploadImage { get; set; }

        public SelectList BranchList { get; set; }
        public bool CanActivate { get; set; }
        public bool CanEditGroups { get; set; }
        public bool CanManageEvents { get; set; }
        public bool CanViewParticipants { get; set; }
        public bool CanViewTriggers { get; set; }
        public SelectList CategoryList { get; set; }
        public string CategoryPlaceholder { get; set; }
        public Challenge Challenge { get; set; }
        public string CreatedByName { get; set; }
        public ICollection<Trigger> DependentTriggers { get; set; }
        public List<ChallengeGroup> Groups { get; set; }
        public bool IgnorePointLimits { get; set; }
        public int? MaxPointLimit { get; set; }
        public string MaxPointsMessage { get; set; }
        public string MaxPointsWarningMessage { get; set; }
        public SelectList ProgramList { get; set; }
        public List<Event> RelatedEvents { get; set; }
        public SelectList SystemList { get; set; }
        public ChallengeTask Task { get; set; }
        public string TaskFilePath { get; set; }
        public IFormFile TaskUploadFile { get; set; }
        public bool UseBadgeMaker { get; set; }
    }
}
