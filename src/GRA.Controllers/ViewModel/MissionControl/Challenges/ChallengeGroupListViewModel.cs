using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengeGroupListViewModel
    {
        public IEnumerable<GRA.Domain.Model.ChallengeGroup> ChallengeGroups { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public GRA.Domain.Model.ChallengeGroup ChallengeGroup { get; set; }

        public bool CanAddGroups { get; set; }
        public bool CanEditGroups { get; set; }
    }
}
