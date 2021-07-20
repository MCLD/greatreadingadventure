using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int CurrentPointTotal { get; set; }
        public bool SingleEvent { get; set; }
        public string ActivityDescriptionPlural { get; set; }

        public int? ActivityAmount { get; set; }

        [MaxLength(500, ErrorMessage = ErrorMessages.MinLength)]
        [DisplayName(DisplayNames.Title)]
        public string Title { get; set; }

        [MaxLength(255, ErrorMessage = ErrorMessages.MinLength)]
        [DisplayName(DisplayNames.Author)]
        public string Author { get; set; }

        public bool DisableSecretCode { get; set; }

        [DisplayName(DisplayNames.SecretCode)]
        public string SecretCode { get; set; }

        public string SecretCodeMessage { get; set; }

        public bool DailyImageLarge { get; set; }
        public string DailyImageMessage { get; set; }
        public string DailyImagePath { get; set; }

        public string DashboardPageContent { get; set; }

        public int? ActivityEarned { get; set; }
        public int? TotalProgramGoal { get; set; }
        public int? PercentComplete { get; set; }
        public string ProgressMessage { get; set; }

        public ICollection<UserLog> UserLogs { get; set; }

        public ICollection<AvatarElement> AvatarElements { get; set; }

        public string AvatarDescription { get; set; }

        public string ProgramName { get; set; }
        public bool UserJoined { get; set; }
        public bool FirstTimeParticipant { get; set; }

        public Carousel Carousel { get; set; }
        public SiteStage SiteStage { get; set; }

        public bool HasPendingVendorCodeQuestion { get; set; }
        public DateTime? VendorCodeExpiration { get; set; }
        public ICollection<Event> UpcomingStreams { get; set; }
    }
}
