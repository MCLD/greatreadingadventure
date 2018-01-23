using GRA.Controllers.ViewModel.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class PrizeListViewModel : ParticipantPartialViewModel
    {
        public IEnumerable<GRA.Domain.Model.PrizeWinner> PrizeWinners { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? HeadOfHouseholdId { get; set; }
    }
}
