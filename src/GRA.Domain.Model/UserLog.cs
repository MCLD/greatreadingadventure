﻿using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class UserLog : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        public int? PointTranslationId { get; set; }

        public int? ActivityEarned { get; set; }

        [Required]
        public int PointsEarned { get; set; }

        public int? AwardedBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public bool? HasBeenViewed { get; set; }
        public int? DeletedBy { get; set; }
        public int? ChallengeId { get; set; }
        public int? BadgeId { get; set; }
        public int? EventId { get; set; }
        public int? TriggerId { get; set; }
        public string BadgeFilename { get; set; }

        [MaxLength(255)]
        public string BadgeAltText { get; set; }
        public string Description { get; set; }

        public int? AvatarBundleId { get; set; }
        public int? AvatarBundleItemCount { get; set; }
    }
}
