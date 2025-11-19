using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Drawing : Abstract.BaseDomainEntity
    {
        public DrawingCriterion DrawingCriterion { get; set; }

        [Required]
        [DisplayName("Criteria")]
        public int DrawingCriterionId { get; set; }

        public bool IsArchived { get; set; }

        [DisplayName("Prize Name")]
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [DisplayName("Message Content")]
        [MaxLength(2000)]
        public string NotificationMessage { get; set; }

        public bool NotificationSent { get; set; }

        [DisplayName("Message Subject")]
        [MaxLength(255)]
        public string NotificationSubject { get; set; }

        [MaxLength(1000)]
        [DisplayName("Redemption Instructions for staff")]
        public string RedemptionInstructions { get; set; }

        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public int RelatedSystemId { get; set; }
        [DisplayName("# of Winners")]
        [Range(1, int.MaxValue, ErrorMessage = "There must be at least 1 winner")]
        public int WinnerCount { get; set; }
        public IEnumerable<PrizeWinner> Winners { get; set; }
    }
}
