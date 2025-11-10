using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ReportCriterion : Abstract.BaseDomainEntity
    {
        public string BadgeRequiredList { get; set; }
        public int? BranchId { get; set; }
        public string ChallengeRequiredList { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        public bool Favorite { get; set; }
        public int? GroupInfoId { get; set; }
        public bool IncludeAchieverStatus { get; set; }
        public bool IsFirstTimeParticipant { get; set; }

        [DisplayName("Last Login Before")]
        public DateTime? LastLoginBefore {get;set; }

        public int? MaximumAllowableActivity { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public int? ProgramId { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int? SiteId { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        public int? SystemId { get; set; }
        public string TriggerList { get; set; }
        public int? VendorCodeTypeId { get; set; }
    }
}
