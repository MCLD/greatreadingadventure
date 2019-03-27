using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.SignIn
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Username)]
        public string Username { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }
}
