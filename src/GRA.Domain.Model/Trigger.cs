using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Trigger : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int RelatedSystemId { get; set; }
        public int RelatedBranchId { get; set; }

        public int Points { get; set; }
        [MaxLength(50)]
        public string SecretCode { get; set; }
        public ICollection<int> BadgeIds { get; set; }
        public ICollection<int> ChallengeIds { get; set; }


        public int? LimitToSystemId { get; set; }
        public int? LimitToBranchId { get; set; }
        public int? LimitToProgramId { get; set; }

        [Required]
        [MaxLength(255)]
        public string AwardMessage { get; set; }
        [Required]
        public int AwardBadgeId { get; set; }
        public string AwardBadgeFilename { get; set; }
        public int? AwardVendorCodeTypeId { get; set; }
        public int AwardPoints { get; set; }
    }
}