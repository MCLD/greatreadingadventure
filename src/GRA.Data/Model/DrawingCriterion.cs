using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DrawingCriterion : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public bool IncludePreviousWinners { get; set; }
        public bool IncludeAnyWinners { get; set; }
        public int? ProgramId { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? PointsMinimum { get; set; }
        public int? PointsMaximum { get; set; }
        public DateTime? StartOfPeriod { get; set; }
        public DateTime? EndOfPeriod { get; set; }
        public int? ActivityAmount { get; set; }
        public int? PointTranslationId { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationMessage { get; set; }
    }
}
