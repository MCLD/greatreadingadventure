using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ChallengeTask : Abstract.BaseDomainEntity
    {
        [Required]
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
        [Required]
        public int Position { get; set; }

        [Required]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Author { get; set; }

        [DisplayName("ISBN")]
        [MaxLength(30)]
        public string Isbn { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }

        public int ChallengeTaskTypeId { get; set; }

        [DisplayName("Task Type")]
        public ChallengeTaskType ChallengeTaskType { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? ActivityCount { get; set; }
        public int? PointTranslationId { get; set; }
        public string Description { get; set; }
    }
}
