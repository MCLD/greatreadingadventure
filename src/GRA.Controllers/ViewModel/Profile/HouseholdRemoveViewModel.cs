using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdRemoveViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string MemberUsername { get; set; }
        public string HouseholdTitle { get; set; }

        [Required]
        [MaxLength(36)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
