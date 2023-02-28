using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PasswordResetViewModel : ParticipantPartialViewModel
    {
        public PasswordResetViewModel()
        {
        }

        public PasswordResetViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public int? HeadOfHouseholdId { get; set; }

        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
    }
}
