using System;

namespace GRA.Domain.Model
{
    public class PsDates : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public Site Site { get; set; }

        public DateTime? PerformerRegistrationOpen { get; set; }
        public DateTime? PerformerRegistrationClosed { get; set; }
        public DateTime? PerformerSchedulingPreview { get; set; }
        public DateTime? PerformerSchedulingOpen { get; set; }
        public DateTime? PerformerSchedulingClosed { get; set; }
        public DateTime? PerformerSchedulePosted { get; set; }

        public DateTime? PerformerScheduleStartDate { get; set; }
        public DateTime? PerformerScheduleEndDate { get; set; }
    }
}
