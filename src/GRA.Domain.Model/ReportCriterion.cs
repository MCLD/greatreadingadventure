using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ReportCriterion : Abstract.BaseDomainEntity
    {
        public int? SiteId { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public int? SchoolDistrictId { get; set; }
        public bool Favorite { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        public string BadgeRequiredList { get; set; }
        public string ChallengeRequiredList { get; set; }
    }
}
