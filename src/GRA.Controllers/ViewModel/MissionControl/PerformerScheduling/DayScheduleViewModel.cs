using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class DayScheduleViewModel
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<PsBranchSelection> BranchSelections { get; set; }
    }
}
