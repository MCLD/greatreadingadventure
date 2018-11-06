using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.PerformerRegistration.Home
{
    public class ScheduleViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PsBlackoutDate> BlackoutDates { get; set; }
        public List<PsPerformerSchedule> ScheduleDates { get; set; }
        public bool EditingSchedule { get; set; }

        public string JsonSchedule { get; set; }
    }
}
