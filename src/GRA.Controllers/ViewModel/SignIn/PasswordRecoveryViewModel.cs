using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.SignIn
{
    public class PasswordRecoveryViewModel
    {
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Username)]
        public string Username { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.Token)]
        public string Token { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.NewPassword)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [Compare(nameof(NewPassword), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName(DisplayNames.ConfirmNewPassword)]
        public string ConfirmPassword { get; set; }
    }
}
