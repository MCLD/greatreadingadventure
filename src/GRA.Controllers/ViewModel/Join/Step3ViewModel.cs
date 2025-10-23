using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step3ViewModel
    {
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Username)]
        [MaxLength(36, ErrorMessage = ErrorMessages.MaxLength)]
        public string Username { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [Compare(nameof(Password), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName(DisplayNames.ConfirmPassword)]
        public string ConfirmPassword { get; set; }

        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress(ErrorMessage = Annotations.Validate.Email)]
        [MaxLength(254, ErrorMessage = ErrorMessages.MaxLength)]
        public string Email { get; set; }

        [DisplayName(DisplayNames.PhoneNumber)]
        [Phone(ErrorMessage = Annotations.Validate.Phone)]
        [MaxLength(15, ErrorMessage = ErrorMessages.MaxLength)]
        public string PhoneNumber { get; set; }

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

        public bool AskPersonalPointGoal { get; set; }

        public int? MaximumPersonalPointGoal { get; set; }
        public int? MinimumPersonalPointGoal { get; set; }

        [DisplayName(DisplayNames.PersonalPointGoal)]
        public int? PersonalPointGoal { get; set; }

        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }
    }
}
