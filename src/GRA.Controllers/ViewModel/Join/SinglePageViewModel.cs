using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class SinglePageViewModel : SchoolSelectionViewModel
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

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName("First Name")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName("Last Name")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [DisplayName("Zip Code")]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName("System")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.System)]
        public int? SystemId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName("Branch")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.Branch)]
        public int? BranchId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName("Program")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.ProgramSelection)]
        public int? ProgramId { get; set; }

        public int? Age { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }

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

        public string AuthorizationCode { get; set; }
    }
}
