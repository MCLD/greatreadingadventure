using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Drawing : Abstract.BaseDomainEntity
    {
        [Required]
        [DisplayName("Criteria")]
        public int DrawingCriterionId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [DisplayName("Prize Name")]
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        [DisplayName("Redemption Instructions")]
        public string RedemptionInstructions { get; set; }
        [DisplayName("# of Winners")]
        [Range(1, Int32.MaxValue, ErrorMessage = "There must be at least 1 winner")]
        public int WinnerCount { get; set; }
        [DisplayName("Mail Subject")]
        public string NotificationSubject { get; set; }
        [DisplayName("Mail Message")]
        public string NotificationMessage { get; set; }
        public bool IsArchived { get; set; }

        public DrawingCriterion DrawingCriterion { get; set; }
        public IEnumerable<PrizeWinner> Winners { get; set; }
    }
}
