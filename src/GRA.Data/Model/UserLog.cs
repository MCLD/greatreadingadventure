using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserLog : Abstract.BaseDbEntity
    {
        public int? ActivityEarned { get; set; }

        public int? AttachmentId { get; set; }

        public int? AvatarBundleId { get; set; }

        public int? AwardedBy { get; set; }

        public int? BadgeId { get; set; }

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
