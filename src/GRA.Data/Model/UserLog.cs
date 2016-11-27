using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserLog : Abstract.BaseDbEntity
    {
        [Required]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int PointTranslationId { get; set; }

        public int? ActivityEarned { get; set; }
        [Required]
        public int PointsEarned { get; set; }
        public int? AwardedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public int ChallengeId { get; set; }
    }
}
