using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerCalendarViewModel
    {
        public PsBranchSelection BranchSelection { get; set; }
        public PsSettings Settings { get; set; }

        public PsPerformer Performer { get; set; }
        public ICollection<PsBlackoutDate> BlackoutDates { get; set; }
        public IEnumerable<DateTime> BookedDates { get; set; }

        public DayScheduleViewModel DayScheduleModel { get; set; }
    }
}
