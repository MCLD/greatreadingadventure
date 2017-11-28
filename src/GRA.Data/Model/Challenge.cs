using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Challenge : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool IsValid { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int PointsAwarded { get; set; }
        public int TasksToComplete { get; set; }

        public int? LimitToSystemId { get; set; }
        public int? LimitToBranchId { get; set; }
        public int? LimitToProgramId { get; set; }


        public virtual ICollection<ChallengeCategory> ChallengeCategories { get; set; }
        public virtual ICollection<ChallengeTask> Tasks { get; set; }
        public int? BadgeId { get; set; }

        public UserFavoriteChallenge UserChallengeFavorite { get; set; }
    }
}
