using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class UserLog : Abstract.BaseDomainEntity
    {
        public int? ActivityEarned { get; set; }

        public string AttachmentFilename { get; set; }

        public int? AttachmentId { get; set; }

        public bool AttachmentIsCertificate { get; set; }

        public int? AvatarBundleId { get; set; }

        public int? AvatarBundleItemCount { get; set; }

        public int? AwardedBy { get; set; }

        [MaxLength(255)]
        public string BadgeAltText { get; set; }

        public string BadgeFilename { get; set; }

        public int? BadgeId { get; set; }

        public int? BookId { get; set; }

        public int? ChallengeId { get; set; }

        public int? DeletedBy { get; set; }

        public string Description { get; set; }

        public int? EventId { get; set; }

        public bool? HasBeenViewed { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public int PointsEarned { get; set; }

        public int? PointTranslationId { get; set; }

        public int? TriggerId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
