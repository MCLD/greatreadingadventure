using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step3ViewModel
    {
        [Required(ErrorMessage = Annotations.RequiredField)]
        [MaxLength(36)]
        public string Username { get; set; }

        [Required(ErrorMessage = Annotations.RequiredField)]
        public string Password { get; set; }

        [Required(ErrorMessage = Annotations.RequiredField)]
        [Compare("Password", ErrorMessage = Annotations.ValidatePasswordsMatch)]
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
        [Required(ErrorMessage = Annotations.ValidateFirstTime)]
        public string IsFirstTime { get; set; }

        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }

        [Required(ErrorMessage = Annotations.ValidateEmailSubscription)]
        public string EmailSubscriptionRequested { get; set; }

        [DisplayName("Set a personal goal")]
        public int? DailyPersonalGoal { get; set; }

        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }
    }
}
