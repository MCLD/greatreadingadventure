using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.SignIn
{
    public class SignInViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }
}
