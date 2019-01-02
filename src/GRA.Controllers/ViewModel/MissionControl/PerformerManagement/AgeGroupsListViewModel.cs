using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class AgeGroupsListViewModel : PerformerManagementPartialViewModel
    {
        public ICollection<PsAgeGroup> AgeGroups { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PsAgeGroup AgeGroup { get; set; }

        public List<Domain.Model.System> Systems { get; set; }
        public string BackToBackBranchesString { get; set; }
    }
}
