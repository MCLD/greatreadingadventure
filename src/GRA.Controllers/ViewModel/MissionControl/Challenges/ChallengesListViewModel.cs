using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Challenges
{
    public class ChallengesListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Challenge> Challenges { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public string FilterBy { get; set; }
        public int? FilterId { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public bool ShowSystem { get; set; }
        public bool CanAddChallenges { get; set; }
        public bool CanDeleteChallenges { get; set; }
        public bool CanEditChallenges { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
    }
}
