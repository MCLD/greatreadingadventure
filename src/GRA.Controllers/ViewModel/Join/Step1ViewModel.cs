using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step1ViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.FirstName)]
        [MaxLength(255, ErrorMessage = Annotations.Validate.MaxLength)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName(DisplayNames.LastName)]
        [MaxLength(255, ErrorMessage = Annotations.Validate.MaxLength)]
        public string LastName { get; set; }

        [DisplayName(DisplayNames.ZipCode)]
        [MaxLength(32, ErrorMessage = Annotations.Validate.MaxLength)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName(DisplayNames.System)]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.System)]
        public int? SystemId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName(DisplayNames.Branch)]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.Branch)]
        public int? BranchId { get; set; }

        public bool RequirePostalCode { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
