using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.SignIn
{
    public class PasswordRecoveryViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Username)]
        public string Username { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Token)]
        public string Token { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.NewPassword)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [Compare(nameof(NewPassword), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [DisplayName(DisplayNames.ConfirmNewPassword)]
        public string ConfirmPassword { get; set; }
    }
}
