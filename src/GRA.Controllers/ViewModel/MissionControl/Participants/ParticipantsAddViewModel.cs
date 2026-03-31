using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ParticipantsAddViewModel : SchoolSelectionViewModel
    {
        public string ActivityDescriptionPlural { get; set; }
        public int? Age { get; set; }
        public SelectList AskEmailSubscription { get; set; }
        public string AskEmailSubscriptionText { get; set; }
        public SelectList AskFirstTime { get; set; }
        public bool AskPersonalPointGoal { get; set; }

        [Required]
        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldBranch)]
        public int? BranchId { get; set; }

        public SelectList BranchList { get; set; }

        [DisplayName(DisplayNames.DailyPersonalGoal)]
        public int? DailyPersonalGoal { get; set; }

        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress]
        [MaxLength(254)]
        [RequiredIf(nameof(IsEmailRequired), true, ErrorMessage = ErrorMessages.Field)]
        public string Email { get; set; }

        [Required(ErrorMessage = Annotations.Validate.EmailSubscription)]
        public string EmailSubscriptionRequested { get; set; }

        [DisplayName(DisplayNames.FirstName)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [MaxLength(255)]
        public string FirstName { get; set; }

        public bool IsEmailRequired { get; set; }

        [DisplayName(DisplayNames.IsFirstTime)]
        [Required(ErrorMessage = Annotations.Validate.FirstTime)]
        public string IsFirstTime { get; set; }

        [DisplayName(DisplayNames.LastName)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [MaxLength(255)]
        public string LastName { get; set; }

        public int? MaximumPersonalPointGoal { get; set; }
        public int? MinimumPersonalPointGoal { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName(DisplayNames.PersonalPointGoal)]
        public int? PersonalPointGoal { get; set; }

        [DisplayName(DisplayNames.PhoneNumber)]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName(DisplayNames.Program)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldProgram)]
        public int? ProgramId { get; set; }

        public string ProgramJson { get; set; }
        public SelectList ProgramList { get; set; }
        public bool RequirePostalCode { get; set; }

        public bool ShowAge { get; set; }

        public bool ShowSchool { get; set; }

        [Required]
        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldSystem)]
        public int? SystemId { get; set; }

        public SelectList SystemList { get; set; }

        public string TranslationDescriptionPastTense { get; set; }

        [Required]
        [MaxLength(36)]
        public string Username { get; set; }
    }
}
