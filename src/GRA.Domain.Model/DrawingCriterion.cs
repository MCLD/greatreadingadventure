using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DrawingCriterion : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [DisplayName("Program")]
        public int? ProgramId { get; set; }
        [DisplayName("System")]
        public int? SystemId { get; set; }
        [DisplayName("Branch")]
        public int? BranchId { get; set; }
        [DisplayName("Minimum Points")]
        public int? PointsMinimum { get; set; }
        [DisplayName("Maximum Points")]
        public int? PointsMaximum { get; set; }
        [DisplayName("Selection Start")]
        public DateTime? StartOfPeriod { get; set; }
        [DisplayName("Selection End")]
        public DateTime? EndOfPeriod { get; set; }
        [DisplayName("Read A Book")]
        public bool ReadABook { get; set; }
        [DisplayName("Include Admin")]
        public bool IncludeAdmin { get; set; }
        [DisplayName("Exclude Previous Winners")]
        public bool ExcludePreviousWinners { get; set; }

        public string SystemName { get; set; }
        public string BranchName { get; set; }
        [DisplayName("Eligible Count")]
        public int EligibleCount { get; set; }

        public ICollection<int> ProgramIds { get; set; }
    }
}
