using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PrizeListViewModel : ParticipantPartialViewModel
    {
        public PrizeListViewModel()
        {
        }

        public PrizeListViewModel(ParticipantPartialViewModel viewModel) : base(viewModel)
        {
        }

        public bool CanEditDetails { get; set; }
        public EmailAwardViewModel EmailAwardModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public IEnumerable<Domain.Model.PrizeWinner> PrizeWinners { get; set; }
        public Domain.Model.User User { get; set; }
    }
}
