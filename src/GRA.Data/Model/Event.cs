using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class Event : Abstract.BaseDbEntity
    {
        [Required]
        public bool AllDay { get; set; }

        public int? AtBranchId { get; set; }

        public int? AtLocationId { get; set; }

        public virtual Challenge Challenge { get; set; }

        public virtual ChallengeGroup ChallengeGroup { get; set; }

        public int? ChallengeGroupId { get; set; }

        public int? ChallengeId { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        public DateTime? EndDate { get; set; }

        [NotMapped]
        public double EventLocationDistance { get; set; }

        [NotMapped]
        public string EventLocationName { get; set; }

        [MaxLength(300)]
        public string ExternalLink { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsCommunityExperience { get; set; }

        public bool IsStreaming { get; set; }

        public bool IsStreamingEmbed { get; set; }

        [Required]
        public bool IsValid { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? ParentEventId { get; set; }

        public int? ProgramId { get; set; }

        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public int RelatedSystemId { get; set; }

        public int? RelatedTriggerId { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? StreamingAccessEnds { get; set; }

        [MaxLength(2000)]
        public string StreamingLinkData { get; set; }
    }
}