using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HistoryListViewModel : ParticipantPartialViewModel
    {
        public List<HistoryItemViewModel> Historys { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool CanRemoveHistory { get; set; }
        public int TotalPoints { get; set; }
    }
}
