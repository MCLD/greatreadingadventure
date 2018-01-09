using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class EventsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Event> Events { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public string Search { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public bool? Mine { get; set; }
        public string ActiveNav { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }
        public bool CanManageLocations { get; set; }
        public bool CommunityExperience { get; set; }
        public bool RequireSecretCode { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
        public IEnumerable<GRA.Domain.Model.Program> ProgramList { get; set; }
    }
}
