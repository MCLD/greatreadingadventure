using System;

namespace GRA.Domain.Model
{
    public class PsPerformerSchedule : Abstract.BaseDomainEntity
    {
        public int PerformerId { get; set; }
        public PsPerformer Performer { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
