using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DrawingCriterion : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int? ProgramId { get; set; }
        public int? SystemId { get; set; }
        public virtual System System { get; set; }
        public int? BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public int? PointsMinimum { get; set; }
        public int? PointsMaximum { get; set; }
        public DateTime? StartOfPeriod { get; set; }
        public DateTime? EndOfPeriod { get; set; }
        public int? ActivityAmount { get; set; }
        public int? PointTranslationId { get; set; }
        public bool IncludeAdmin { get; set; }
        public bool ExcludePreviousWinners { get; set; }
    }
}
