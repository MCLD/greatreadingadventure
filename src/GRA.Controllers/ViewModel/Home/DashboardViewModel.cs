using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Home
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int CurrentPointTotal { get; set; }
        public string AvatarPath { get; set; }
        public bool SingleEvent { get; set; }
        public string ActivityDescriptionPlural { get; set; }

        public int? ActivityAmount { get; set; }
        [MaxLength(500)]
        public string Title { get; set; }
        [MaxLength(255)]
        public string Author { get; set; }

        public bool DisableSecretCode { get; set; }
        [DisplayName("Secret Code")]
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }

        public string DailyImageMessage { get; set; }
        public string DailyImagePath { get; set; }

        public string DashboardPageContent { get; set; }

        public int? ActivityEarned { get; set; }
        public int? TotalProgramGoal { get; set; }
        public int? PercentComplete { get; set; }

        public IEnumerable<GRA.Domain.Model.Badge> Badges { get; set; }
        public ICollection<GRA.Domain.Model.DynamicAvatarElement> DynamicAvatarElements { get; set; }
    }
}
