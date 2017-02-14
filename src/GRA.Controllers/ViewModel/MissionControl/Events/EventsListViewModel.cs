using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Events
{
    public class EventsListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Event> Events { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public bool CanManageLocations { get; set; }
    }
}
