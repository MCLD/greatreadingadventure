using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Notification : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Text { get; set; }
        public int? BadgeId { get; set; }
        public string BadgeFilename { get; set; }
        public int? ChallengeId { get; set; }
        public int? PointsEarned { get; set; }
        public bool IsAchiever { get; set; }
        public bool IsJoining { get; set; }
    }
}
