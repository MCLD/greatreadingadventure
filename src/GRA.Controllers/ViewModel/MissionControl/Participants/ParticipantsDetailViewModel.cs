using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class ParticipantsDetailViewModel
    {
        public GRA.Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanEditDetails { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
    }
}
