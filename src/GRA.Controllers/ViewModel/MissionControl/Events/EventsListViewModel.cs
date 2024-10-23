using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class EventsListViewModel
    {
        public string ActionTarget
        {
            get
            {
                return CommunityExperience
                    ? nameof(EventsController.CommunityExperiences)
                    : Streaming
                        ? nameof(EventsController.StreamingEvents)
                        : nameof(EventsController.Index);
            }
        }

        public string ActiveNav { get; set; }
        public int? BranchId { get; set; }
        public IEnumerable<Branch> BranchList { get; set; }
        public string BranchName { get; set; }
        public bool CanManageLocations { get; set; }
        public bool CommunityExperience { get; set; }
        public IEnumerable<Event> Events { get; set; }
        public bool? Mine { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public int? ProgramId { get; set; }
        public IEnumerable<Program> ProgramList { get; set; }
        public string ProgramName { get; set; }
        public bool RequireSecretCode { get; set; }
        public string Search { get; set; }
        public bool Streaming { get; set; }
        public int? SystemId { get; set; }
        public IEnumerable<Domain.Model.System> SystemList { get; set; }
        public string SystemName { get; set; }
    }
}
