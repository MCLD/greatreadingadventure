using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.Join
{
    public class JoinViewModel
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
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required]
        public int? BranchId { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        public int ProgramId { get; set; }

        public int SiteId { get; set; }
        
        public int SystemId { get; set; }

        public SelectList BranchList { get; set; }
    }
}
