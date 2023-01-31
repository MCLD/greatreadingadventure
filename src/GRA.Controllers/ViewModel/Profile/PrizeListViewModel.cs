using System.Collections.Generic;
using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.Profile
{
    public class PrizeListViewModel : ParticipantPartialViewModel
    {
        public int? HeadOfHouseholdId { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public IEnumerable<Domain.Model.PrizeWinner> PrizeWinners { get; set; }
    }
}
