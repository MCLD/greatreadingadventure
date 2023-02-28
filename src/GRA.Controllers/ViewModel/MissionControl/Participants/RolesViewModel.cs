using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class RolesViewModel : ParticipantPartialViewModel
    {
        public RolesViewModel()
        {
        }

        public RolesViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public string Roles { get; set; }
        public IEnumerable<Role> SelectedRoles { get; set; }
        public IEnumerable<Role> UnselectedRoles { get; set; }
    }
}
