using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class EventsDetailViewModel
    {
        public GRA.Domain.Model.Event Event { get; set; }
        public bool UseLocation { get; set; }

        public bool CanManageLocations { get; set; }
        public GRA.Domain.Model.Location Location { get; set; }

        public SelectList BranchList { get; set; }
        public SelectList LocationList { get; set; }
        public SelectList ProgramList { get; set; }
    }
}