using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Challenge : Abstract.BaseDomainEntity
    {
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

        [Required]
        [DisplayName("Points Awarded")]
        [Range(1, int.MaxValue, ErrorMessage = "The minimum points that can be awarded is {1}")]
        public int? PointsAwarded { get; set; }

        [Required]
        [DisplayName("Tasks To Complete")]
        [Range(1, int.MaxValue, ErrorMessage = "The minimum tasks required to complete is {1}")]
        public int? TasksToComplete { get; set; }

        [DisplayName("Limit to System?")]
        public int? LimitToSystemId { get; set; }
        [DisplayName("Limit to Branch?")]
        public int? LimitToBranchId { get; set; }
        [DisplayName("Limit to Program?")]
        public int? LimitToProgramId { get; set; }

        public IEnumerable<ChallengeTask> Tasks { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; }
        public int? PercentComplete { get; set; }
        public int? CompletedTasks { get; set; }
        public int? BadgeId { get; set; }
        public string BadgeFilename { get; set; }
        public bool HasDependents { get; set; }
        public bool IsFavorited { get; set; }

        [DisplayName("Categories")]
        public ICollection<int> CategoryIds { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
