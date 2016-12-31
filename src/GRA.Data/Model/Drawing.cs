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

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string RedemptionInstructions { get; set; }
        [Range(1, Int32.MaxValue)]
        public int WinnerCount { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationMessage { get; set; }

        public virtual ICollection<DrawingWinner> Winners { get; set; }
    }
}
