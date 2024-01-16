using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class PsBranchSelection : Abstract.BaseDbEntity
    {
        public PsAgeGroup AgeGroup { get; set; }
        public int AgeGroupId { get; set; }
        public bool BackToBackProgram { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User CreatedByUser { get; set; }

        public bool IsDeleted { get; set; }
        public PsKit Kit { get; set; }
        public int? KitId { get; set; }
        public PsProgram Program { get; set; }
        public int? ProgramId { get; set; }
        public DateTime RequestedStartTime { get; set; }
        public int ScheduleDuration { get; set; }
        public DateTime ScheduleStartTime { get; set; }

        [MaxLength(50)]
        public string SecretCode { get; set; }

        public DateTime SelectedAt { get; set; }
        public User? UpdatedByUser { get; set; }
        public int? UpdatedByUserId { get; set; }

        [MaxLength(255)]
        public string OnSiteContactName { get; set; }
        [MaxLength(255)]
        public string OnSiteContactPhone { get; set; }
        [MaxLength(255)]
        public string OnSiteContactEmail { get; set; }
    }
}
