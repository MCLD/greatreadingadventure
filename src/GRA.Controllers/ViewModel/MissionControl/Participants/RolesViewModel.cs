using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class RolesViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<Role> SelectedRoles { get; set; }
        public IEnumerable<Role> UnselectedRoles { get; set; }
        public string Roles { get; set; }
    }
}
