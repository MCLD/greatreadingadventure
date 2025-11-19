using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class ChangePasswordViewModel
    {
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.OldPassword)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.NewPassword)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.ConfirmNewPassword)]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }
    }
}
