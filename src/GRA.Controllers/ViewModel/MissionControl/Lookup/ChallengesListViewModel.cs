using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Lookup
{
    public class ChallengesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool CanEditChallenges { get; set; }
        public bool ShowActive { get; set; }
    }
}
