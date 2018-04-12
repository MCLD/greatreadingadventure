using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class Trigger : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int Points { get; set; }
        [MaxLength(50)]
        public string SecretCode { get; set; }
        public int ItemsRequired { get; set; }

        public ICollection<TriggerBadge> RequiredBadges { get; set; }
        public ICollection<TriggerChallenge> RequiredChallenges { get; set; }

        public int? LimitToSystemId { get; set; }
        [ForeignKey("LimitToSystemId")]
        public System LimitToSystem { get; set; }
        public int? LimitToBranchId { get; set; }
        [ForeignKey("LimitToBranchId")]
        public Branch LimitToBranch { get; set; }
        public int? LimitToProgramId { get; set; }
        [ForeignKey("LimitToProgramId")]
        public Program LimitToProgram { get; set; }

        [Required]
        [MaxLength(1000)]
        public string AwardMessage { get; set; }
        [Required]
        public int AwardBadgeId { get; set; }
        [ForeignKey("AwardBadgeId")]
        public Badge AwardBadge { get; set; }
        public int? AwardVendorCodeTypeId { get; set; }
        public VendorCodeType AwardVendorCodeType { get; set; }
        public int AwardPoints { get; set; }
        [MaxLength(500)]
        public string AwardMailSubject { get; set; }
        [MaxLength(2000)]
        public string AwardMail { get; set; }
        [MaxLength(255)]
        public string AwardPrizeName { get; set; }
        [MaxLength(1000)]
        public string AwardPrizeRedemptionInstructions { get; set; }
        public int? AwardAvatarBundleId { get; set; }
        public AvatarBundle AwardAvatarBundle { get; set; }
    }
}
