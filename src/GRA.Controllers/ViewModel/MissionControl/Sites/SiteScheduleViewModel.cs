using System;
using System.ComponentModel;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteScheduleViewModel
    {
        public int Id { get; set; }

        public bool CollectPreregistrationEmails { get; set; }
        [DisplayName("Before RegistrationPage")]
        public int? BeforeRegistrationPage { get; set; }
        [DisplayName("Registration Opens")]
        public DateTime? RegistrationOpens { get; set; }
        [DisplayName("Registration Open Page")]
        public int? RegistrationOpenPage { get; set; }
        [DisplayName("Program Starts")]
        public DateTime? ProgramStarts { get; set; }
        [DisplayName("Program Starts Page")]
        public int? ProgramOpenPage { get; set; }
        [DisplayName("Program Ends")]
        public DateTime? ProgramEnds { get; set; }
        [DisplayName("Program Ended Page")]
        public int? ProgramEndedPage { get; set; }
        [DisplayName("Access Closed")]
        public DateTime? AccessClosed { get; set; }
        [DisplayName("Access Closed Page")]
        public int? AccessClosedPage { get; set; }
    }
}
