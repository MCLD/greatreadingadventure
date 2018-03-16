using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Join
{
    public class Step3ViewModel
    {
        [Required]
        [MaxLength(36)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Email Address")]
        [EmailAddress]
        [MaxLength(254)]
        public string Email { get; set; }

        [DisplayName("Phone Number")]
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public SelectList AskFirstTime { get; set; }

        [DisplayName("Is this your first time participating?")]
        [Required(ErrorMessage = "Please let us know if this is your first time participating in the program")]
        public string IsFirstTime { get; set; }

        public bool AskEmailReminder { get; set; }
        [DisplayName("Would you like for us to send an email reminder when the program starts?")]
        public bool PreregistrationReminderRequested { get; set; }

        [DisplayName("Set a personal goal")]
        public int? DailyPersonalGoal { get; set; }
        public string TranslationDescriptionPastTense { get; set; }
        public string ActivityDescriptionPlural { get; set; }
    }
}
