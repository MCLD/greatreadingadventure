using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class SinglePageViewModel : SchoolSelectionViewModel
    {
        public string ActivityDescriptionPlural { get; set; }

        [DisplayName(DisplayNames.Age)]
        public int? Age { get; set; }

        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }
        public SelectList AskFirstTime { get; set; }
        public string AuthorizationCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldBranch)]
        public int? BranchId { get; set; }

        public SelectList BranchList { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [Compare(nameof(Password), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName(DisplayNames.ConfirmPassword)]
        public string ConfirmPassword { get; set; }

        [DisplayName(DisplayNames.DailyPersonalGoal)]
        public int? DailyPersonalGoal { get; set; }

        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress(ErrorMessage = Annotations.Validate.Email)]
        [MaxLength(254, ErrorMessage = ErrorMessages.MaxLength)]
        public string Email { get; set; }

        [Required(ErrorMessage = Annotations.Validate.EmailSubscription)]
        public string EmailSubscriptionRequested { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.FirstName)]
        [MaxLength(255, ErrorMessage = ErrorMessages.MaxLength)]
        public string FirstName { get; set; }

        [DisplayName(DisplayNames.IsFirstTime)]
        [Required(ErrorMessage = Annotations.Validate.FirstTime)]
        public string IsFirstTime { get; set; }

        public string JoinCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.LastName)]
        [MaxLength(255, ErrorMessage = ErrorMessages.MaxLength)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Password)]
        public string Password { get; set; }

        [DisplayName(DisplayNames.PhoneNumber)]
        [Phone(ErrorMessage = Annotations.Validate.Phone)]
        [MaxLength(15, ErrorMessage = ErrorMessages.MaxLength)]
        public string PhoneNumber { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32, ErrorMessage = ErrorMessages.MaxLength)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.Program)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldProgram)]
        public int? ProgramId { get; set; }

        public string ProgramJson { get; set; }
        public SelectList ProgramList { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }

        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldSystem)]
        public int? SystemId { get; set; }

        public SelectList SystemList { get; set; }
        public string TranslationDescriptionPastTense { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Username)]
        [MaxLength(36, ErrorMessage = ErrorMessages.MaxLength)]
        public string Username { get; set; }

        public string WelcomeMessage { get; set; }
    }
}
