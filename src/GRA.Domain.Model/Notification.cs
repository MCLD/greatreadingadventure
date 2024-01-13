using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Notification : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        public string AltText { get; set; }

        public string AttachmentFilename { get; set; }

        public int? AvatarBundleId { get; set; }

        public string BadgeFilename { get; set; }

        public int? BadgeId { get; set; }

        public int? ChallengeId { get; set; }

        public string DisplayIcon { get; set; }

        public bool IsAchiever { get; set; }

        public bool IsJoiner { get; set; }

        public object LocalizedText { get; set; }

        public int? PointsEarned { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
