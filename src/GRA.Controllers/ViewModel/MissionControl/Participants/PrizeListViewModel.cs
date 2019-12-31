using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PrizeListViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<GRA.Domain.Model.PrizeWinner> PrizeWinners { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
    }
}
