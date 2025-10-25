using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step1ViewModel
    {
        public string AuthorizationCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldBranch)]
        public int? BranchId { get; set; }

        public SelectList BranchList { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.FirstName)]
        [MaxLength(255, ErrorMessage = ErrorMessages.MaxLength)]
        public string FirstName { get; set; }

        public string JoinCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.LastName)]
        [MaxLength(255, ErrorMessage = ErrorMessages.MaxLength)]
        public string LastName { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32, ErrorMessage = ErrorMessages.MaxLength)]
        public string PostalCode { get; set; }

        public bool RequirePostalCode { get; set; }

        [Required(ErrorMessage = ErrorMessages.Selection)]
        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.FieldSystem)]
        public int? SystemId { get; set; }

        public SelectList SystemList { get; set; }
        public string WelcomeMessage { get; set; }
    }
}
