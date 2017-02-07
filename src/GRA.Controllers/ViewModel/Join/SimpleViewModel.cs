using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Join
{
    public class SinglePageViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Zip Code")]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName("System")]
        [Range(0, int.MaxValue, ErrorMessage = "The Branch field is required.")]
        public int? SystemId { get; set; }

        [Required]
        [DisplayName("Branch")]
        [Range(0, int.MaxValue, ErrorMessage = "The Branch field is required.")]
        public int? BranchId { get; set; }

        [Required]
        [DisplayName("Program")]
        [Range(0, int.MaxValue, ErrorMessage = "The Branch field is required.")]
        public int? ProgramId { get; set; }

        public int? Age { get; set; }
        [DisplayName("School")]
        public int? SchoolId { get; set; }
        [DisplayName("School Name")]
        public string EnteredSchoolName { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public bool NewEnteredSchool { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
        public string ProgramJson { get; set; }

        public SelectList SystemList { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
    }
}
