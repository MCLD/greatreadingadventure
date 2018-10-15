using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class BlackoutDatesViewModel : PerformerManagementPartialViewModel
    {
        public ICollection<PsBlackoutDate> BlackoutDates { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PsBlackoutDate BlackoutDate { get; set; }
    }
}
