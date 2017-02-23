using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Trigger : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int RelatedSystemId { get; set; }
        public int RelatedBranchId { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? Points { get; set; }
        [MaxLength(50)]
        [DisplayName("Secret Code")]
        [RegularExpression("([a-zA-Z0-9]+)", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string SecretCode { get; set; }
        [DisplayName("Items Required")]
        [Range(0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int? ItemsRequired { get; set; }
        public ICollection<int> BadgeIds { get; set; }
        public ICollection<int> ChallengeIds { get; set; }

        [DisplayName("Limit to System?")]
        public int? LimitToSystemId { get; set; }
        [DisplayName("Limit to Branch?")]
        public int? LimitToBranchId { get; set; }
        [DisplayName("Limit to Program?")]
        public int? LimitToProgramId { get; set; }

        [Required]
        [MaxLength(1000)]
        [DisplayName("Award Message")]
        public string AwardMessage { get; set; }
        [Required]
        public int AwardBadgeId { get; set; }
        public string AwardBadgeFilename { get; set; }
        [DisplayName("Award Vendor Code")]
        public int? AwardVendorCodeTypeId { get; set; }
        [DisplayName("Award Points")]
        [Range (0, int.MaxValue, ErrorMessage = "{0} cannot be less than {1}.")]
        public int AwardPoints { get; set; }
        public bool HasDependents { get; set; }
    }
}