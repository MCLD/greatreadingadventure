using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsBranchSelection : Abstract.BaseDomainEntity
    {
        public int UserId { get; set; }

        [DisplayName("Branch")]
        public int BranchId { get; set; }

        [DisplayName("Age Group")]
        public int AgeGroupId { get; set; }

        public int? ProgramId { get; set; }

        [DisplayName("Kit")]
        public int? KitId { get; set; }

        public DateTime RequestedStartTime { get; set; }
        public DateTime ScheduleStartTime { get; set; }
        public int ScheduleDuration { get; set; }
        public bool BackToBackProgram { get; set; }

        public DateTime SelectedAt { get; set; }

        [MaxLength(50)]
        public string SecretCode { get; set; }
    }
}
