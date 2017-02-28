using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HistoryListViewModel
    {
        public List<HistoryItemViewModel> Historys { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanRemoveHistory { get; set; }
        public int TotalPoints { get; set; }
    }
}
