using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PasswordResetViewModel : ParticipantPartialViewModel
    {
        public int? HeadOfHouseholdId { get; set; }
        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
    }
}
