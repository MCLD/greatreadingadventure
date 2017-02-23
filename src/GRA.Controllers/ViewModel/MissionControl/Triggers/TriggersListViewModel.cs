using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Triggers
{
    public class TriggersListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Trigger> Triggers { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
