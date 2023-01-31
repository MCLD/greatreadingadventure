using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Trigger : Abstract.BaseDomainEntity
    {
        [DisplayName("Activation Date")]
        public DateTime? ActivationDate { get; set; }

        public string AwardAttachmentFilename { get; set; }
        public int? AwardAttachmentId { get; set; }
        [DisplayName("Award avatar bundle")]
        public int? AwardAvatarBundleId { get; set; }

        public string AwardBadgeFilename { get; set; }
        [Required]
        public int AwardBadgeId { get; set; }

        [MaxLength(2000)]
        [DisplayName("Mail message")]
        public string AwardMail { get; set; }

        [MaxLength(500)]
        [DisplayName("Mail subject")]
        public string AwardMailSubject { get; set; }

        [Required]
        [MaxLength(1000)]
        [DisplayName("Notification")]
        public string AwardMessage { get; set; }

        [DisplayName("Award points")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int AwardPoints { get; set; }

        [MaxLength(255)]
        [DisplayName("Prize name")]
        [Display(Description = "Reminder: this name is visible to participants.")]
        public string AwardPrizeName { get; set; }

        [MaxLength(1000)]
        [DisplayName("Redemption instructions")]
        public string AwardPrizeRedemptionInstructions { get; set; }

        [DisplayName("Award vendor code")]
        public int? AwardVendorCodeTypeId { get; set; }

        public ICollection<int> BadgeIds { get; set; }
        public ICollection<int> ChallengeIds { get; set; }
        public bool HasDependents { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayName("Items Required")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? ItemsRequired { get; set; }

        [DisplayName("Limit to Branch?")]
        public int? LimitToBranchId { get; set; }

        public string LimitToBranchName { get; set; }
        [DisplayName("Limit to Program?")]
        public int? LimitToProgramId { get; set; }

        [DisplayName("Limit to System?")]
        public int? LimitToSystemId { get; set; }

        public string LimitToSystemName { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Trigger name")]
        public string Name { get; set; }

        [DisplayName("Must have this many points")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? Points { get; set; }

        public int RelatedBranchId { get; set; }
        public int? RelatedEventId { get; set; }
        public string RelatedEventName { get; set; }
        public int RelatedSystemId { get; set; }
        [MaxLength(50)]
        [DisplayName("Secret code")]
        [RegularExpression("([a-zA-Z0-9]+)", ErrorMessage = "Only letters and numbers are allowed.")]
        public string SecretCode { get; set; }

        public int SiteId { get; set; }
    }
}
