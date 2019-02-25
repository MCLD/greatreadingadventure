using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step3ViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [MaxLength(36)]
        public string Username { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        public string Password { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [Compare("Password", ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public SelectList AskFirstTime { get; set; }

        [DisplayName("Is this your first time participating?")]
        [Required(ErrorMessage = Annotations.Validate.FirstTime)]
        public string IsFirstTime { get; set; }

        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }

        [Required(ErrorMessage = Annotations.Validate.EmailSubscription)]
        public string EmailSubscriptionRequested { get; set; }

        [DisplayName("Set a personal goal")]
        public int? DailyPersonalGoal { get; set; }

        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }
    }
}
