using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class SelectProgramViewModel
    {
        public PsBranchSelection BranchSelection { get; set; }
        public PsSettings Settings { get; set; }

        public ICollection<PsBlackoutDate> BlackoutDates { get; set; }
        public ICollection<PsPerformerSchedule> ScheduleDates { get; set; }
        public IEnumerable<DateTime> BookedDates { get; set; }
    }
}
