using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Lookup
{
    public class ChallengeGroupsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.ChallengeGroup> ChallengeGroups { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool CanEditGroups { get; set; }
    }
}
