using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step1ViewModel
    {
        [Required(ErrorMessage = Annotations.RequiredField)]
        [DisplayName("First Name")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = Annotations.RequiredField)]
        [DisplayName("Last Name")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [DisplayName("Zip Code")]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = Annotations.RequiredSelection)]
        [DisplayName("System")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.RequiredSystem)]
        public int? SystemId { get; set; }

        [Required(ErrorMessage = Annotations.RequiredSelection)]
        [DisplayName("Branch")]
        [Range(0, int.MaxValue, ErrorMessage = Annotations.RequiredBranch)]
        public int? BranchId { get; set; }

        public bool RequirePostalCode { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }

        public string AuthorizationCode { get; set; }
    }
}
