using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Home
{
    public class DashboardViewModel
    {
        public int? ActivityAmount { get; set; }
        public string ActivityDescriptionPlural { get; set; }
        public int? ActivityEarned { get; set; }

        [MaxLength(255, ErrorMessage = ErrorMessages.MinLength)]
        [DisplayName(DisplayNames.Author)]
        public string Author { get; set; }

        public ICollection<AvatarElement> AvatarElements { get; set; }
        public string ButtonText { get; set; }
        public Carousel Carousel { get; set; }
        public bool DailyImageLarge { get; set; }
        public string DailyImageMessage { get; set; }
        public string DailyImagePath { get; set; }
        public string DashboardAlert { get; set; }
        public AlertType DashboardAlertType { get; set; }
        public string DashboardPageContent { get; set; }
        public int Day { get; set; }
        public bool DisableSecretCode { get; set; }
        public bool FirstTimeParticipant { get; set; }

        public bool HasOrderStatus
        {
            get
            {
                return User != null
                    && User.VendorOrderStatus > VendorOrderStatus.Pending
                    && User.VendorOrderStatus < VendorOrderStatus.Received;
            }
        }

        public bool HasPendingVendorCodeQuestion { get; set; }
        public bool IsPerformerScheduling { get; set; }
        public int? PercentComplete { get; set; }
        public string ProgramName { get; set; }
        public string ProgressMessage { get; set; }

        [DisplayName(DisplayNames.SecretCode)]
        public string SecretCode { get; set; }

        public string SecretCodeMessage { get; set; }
        public bool SingleEvent { get; set; }
        public int? SiteActivityPercentComplete { get; set; }
        public int? SiteReadingGoal { get; set; }
        public SiteStage SiteStage { get; set; }

        [MaxLength(500, ErrorMessage = ErrorMessages.MinLength)]
        [DisplayName(DisplayNames.Title)]
        public string Title { get; set; }

        public int? TotalProgramGoal { get; set; }
        public int? TotalSiteActivity { get; set; }
        public ICollection<Event> UpcomingStreams { get; set; }
        public User User { get; set; }
        public bool UserJoined { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }
        public DateTime? VendorCodeExpiration { get; set; }

        public static string AlertClass(AlertType alertType) =>
            alertType switch
            {
                AlertType.Warning => "alert-warning",
                AlertType.Danger => "alert-danger",
                AlertType.Success => "alert-success",
                _ => "alert-info"
            };

        public string ActiveStatus(VendorOrderStatus status)
        {
            return User.VendorOrderStatus == status
                ? "text-success"
                : "text-secondary";
        }
    }
}
