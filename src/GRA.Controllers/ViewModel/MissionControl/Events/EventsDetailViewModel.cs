using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class EventsDetailViewModel
    {
        public GRA.Domain.Model.Event Event { get; set; }
        public bool UseLocation { get; set; }
        public bool NewCommunityExperience { get; set; }

        public bool IncludeSecretCode { get; set; }
        [MaxLength(50)]
        [DisplayName("Secret code")]
        [RegularExpression("([a-zA-Z0-9]+)", ErrorMessage = "Only letters and numbers are allowed.")]
        [Required]
        public string SecretCode { get; set; }
        [MaxLength(1000)]
        [DisplayName("Notification")]
        [Required]
        public string AwardMessage { get; set; }
        [DisplayName("Award points")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int AwardPoints { get; set; }

        public IFormFile BadgeUploadImage { get; set; }
        public string BadgeMakerUrl { get; set; }
        public bool UseBadgeMaker { get; set; }
        public string BadgeMakerImage { get; set; }

        public bool CanAddSecretCode { get; set; }
        public bool CanEditGroups { get; set; }
        public bool CanManageLocations { get; set; }
        public bool CanRelateChallenge { get; set; }
        public GRA.Domain.Model.Location Location { get; set; }

        [DisplayName("System")]
        public int SystemId { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList ProgramList { get; set; }
    }
}