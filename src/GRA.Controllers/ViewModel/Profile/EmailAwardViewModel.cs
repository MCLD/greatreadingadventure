using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.Profile
{
    public class EmailAwardViewModel
    {
        public int? UserId { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.EmailAddress)]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        public string EmailAwardInstructions { get; set; }
    }
}
