using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Drawing : Abstract.BaseDbEntity
    {
        [Required]
        public int DrawingCriterionId { get; set; }
        public DrawingCriterion DrawingCriterion { get; set; }

        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string RedemptionInstructions { get; set; }
        [Range(1, Int32.MaxValue)]
        public int WinnerCount { get; set; }
        [MaxLength(255)]
        public string NotificationSubject { get; set; }
        [MaxLength(2000)]
        public string NotificationMessage { get; set; }
        public bool IsArchived { get; set; }

        public virtual ICollection<PrizeWinner> Winners { get; set; }
    }
}
