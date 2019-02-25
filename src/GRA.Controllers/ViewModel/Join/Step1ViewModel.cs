using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step1ViewModel
    {
        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName("First Name")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = Annotations.Required.Field)]
        [DisplayName("Last Name")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [DisplayName("Zip Code")]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName("System")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.System)]
        public int? SystemId { get; set; }

        [Required(ErrorMessage = Annotations.Required.Selection)]
        [DisplayName("Branch")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.Required.Branch)]
        public int? BranchId { get; set; }

        public bool RequirePostalCode { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
