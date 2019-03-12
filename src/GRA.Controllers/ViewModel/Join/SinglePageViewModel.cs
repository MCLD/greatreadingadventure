using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class SinglePageViewModel : SchoolSelectionViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Username)]
        [MaxLength(36, ErrorMessage = Annotations.Validate.MaxLength)]
        public string Username { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [Compare(DisplayNames.Password, ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName(DisplayNames.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.FirstName)]
        [MaxLength(255, ErrorMessage = Annotations.Validate.MaxLength)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.LastName)]
        [MaxLength(255, ErrorMessage = Annotations.Validate.MaxLength)]
        public string LastName { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32, ErrorMessage = Annotations.Validate.MaxLength)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.System)]
        public int? SystemId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.Branch)]
        public int? BranchId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName(DisplayNames.Program)]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.ProgramSelection)]
        public int? ProgramId { get; set; }

        [DisplayName(DisplayNames.Age)]
        public int? Age { get; set; }

        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress(ErrorMessage = Annotations.Validate.Email)]
        [MaxLength(254, ErrorMessage = Annotations.Validate.MaxLength)]
        public string Email { get; set; }

        [DisplayName(DisplayNames.PhoneNumber)]
        [Phone(ErrorMessage = Annotations.Validate.Phone)]
        [MaxLength(15, ErrorMessage = Annotations.Validate.MaxLength)]
        public string PhoneNumber { get; set; }

        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public string ProgramJson { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }

        public SelectList AskFirstTime { get; set; }

        [DisplayName(DisplayNames.IsFirstTime)]
        [Required(ErrorMessage = Annotations.Validate.FirstTime)]
        public string IsFirstTime { get; set; }

        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }

        [Required(ErrorMessage = Annotations.Validate.EmailSubscription)]
        public string EmailSubscriptionRequested { get; set; }

        [DisplayName(DisplayNames.DailyPersonalGoal)]
        public int? DailyPersonalGoal { get; set; }

        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
