using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Profile
{
    public class ChangePasswordViewModel
    {
        public int HouseholdCount { get; set; }
        public bool HasAccount { get; set; }

        [Required]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [DisplayName("Confirm New Password")]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }
    }
}
