using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public abstract class ParticipantPartialViewModel : SchoolSelectionViewModel
    {
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public bool HasAccount { get; set; }
        public bool IsGroup { get; set; }
    }
}
