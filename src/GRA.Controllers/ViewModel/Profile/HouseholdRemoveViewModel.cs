using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

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

        [Compare(nameof(Password), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.ConfirmNewPassword)]
        public string ConfirmPassword { get; set; }
    }
}
