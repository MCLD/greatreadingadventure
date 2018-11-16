using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerScheduleViewModel
    {
        public PsPerformer Performer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<PsBlackoutDate> BlackoutDates { get; set; }
        public List<PsPerformerSchedule> ScheduleDates { get; set; }

        public string JsonSchedule { get; set; }
    }
}
