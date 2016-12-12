using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.SignIn
{
    public class PasswordRecoveryViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [DisplayName("Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
