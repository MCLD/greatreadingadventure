using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PrizeListViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<Domain.Model.PrizeWinner> PrizeWinners { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public Domain.Model.User User { get; set; }
        public bool CanEditDetails { get; set; }

        public EmailAwardViewModel EmailAwardModel { get; set; }
    }
}
