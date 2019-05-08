using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdRegisterViewModel
    {
        public int RegisterId { get; set; }

        [Required]
        [MaxLength(36)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = Annotations.Validate.PasswordsMatch)]
        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.ConfirmNewPassword)]
        public string ConfirmPassword { get; set; }

        public bool Validate { get; set; }
    }
}
