using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsBranchSelection : Abstract.BaseDomainEntity
    {
        public PsAgeGroup AgeGroup { get; set; }

        [DisplayName("Age Group")]
        public int AgeGroupId { get; set; }

        public bool BackToBackProgram { get; set; }
        public Branch Branch { get; set; }

        [DisplayName("Branch")]
        public int BranchId { get; set; }

        public User CreatedByUser { get; set; }
        public string EndsAt { get; set; }
        public bool IsDeleted { get; set; }
        public PsKit Kit { get; set; }

        [DisplayName("Kit")]
        public int? KitId { get; set; }

        public PsProgram Program { get; set; }

        [DisplayName("Program")]
        public int? ProgramId { get; set; }

        public DateTime RequestedStartTime { get; set; }
        public int ScheduleDuration { get; set; }
        public DateTime ScheduleStartTime { get; set; }

        [MaxLength(50)]
        public string SecretCode { get; set; }

        public DateTime SelectedAt { get; set; }
        public string StartsAt { get; set; }
        public string Summary { get; set; }
        public User? UpdatedByUser { get; set; }
        public int? UpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public string OnSiteContactName { get; set; }
        public string OnSiteContactPhone { get; set; }
        public string OnSiteContactEmail { get; set; }
    }
}
