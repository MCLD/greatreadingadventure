using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Event : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int RelatedSystemId { get; set; }
        [Required]
        public int RelatedBranchId { get; set; }
        public int? RelatedTriggerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool AllDay { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsValid { get; set; }
        [Required]
        public bool IsCommunityExperience { get; set; }

        [MaxLength(300)]
        public string ExternalLink { get; set; }

        public int? AtBranchId { get; set; }
        public int? AtLocationId { get; set; }

        public int? ProgramId { get; set; }
        public int? ParentEventId { get; set; }

        public int? ChallengeId { get; set; }
        public virtual Challenge Challenge { get; set; }
        public int? ChallengeGroupId { get; set; }
        public virtual ChallengeGroup ChallengeGroup { get; set; }
    }
}
