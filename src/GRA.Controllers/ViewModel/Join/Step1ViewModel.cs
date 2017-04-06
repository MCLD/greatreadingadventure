using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step1ViewModel
    {
        [Required]
        [DisplayName("First Name")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [MaxLength(255)]
        public string LastName { get; set; }

        [DisplayName("Zip Code")]
        [MaxLength(32)]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName("System")]
        public int? SystemId { get; set; }

        [Required]
        [DisplayName("Branch")]
        public int? BranchId { get; set; }

        public bool RequirePostalCode { get; set; }


        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
    }
}
