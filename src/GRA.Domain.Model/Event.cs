﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Event : Abstract.BaseDomainEntity
    {
        [DisplayName("All Day")]
        [Required]
        public bool AllDay { get; set; }

        [DisplayName("At Branch")]
        public int? AtBranchId { get; set; }

        [DisplayName("At Location")]
        public int? AtLocationId { get; set; }

        public Challenge Challenge { get; set; }
        public ChallengeGroup ChallengeGroup { get; set; }
        public int? ChallengeGroupId { get; set; }
        public int? ChallengeId { get; set; }

        [Required]
        [MaxLength(1500)]
        public string Description { get; set; }

        [DisplayName("End")]
        public DateTime? EndDate { get; set; }

        public string EventLocationAddress { get; set; }
        public double EventLocationDistance { get; set; }
        public string EventLocationLink { get; set; }
        public string EventLocationName { get; set; }
        public string EventLocationTelephone { get; set; }

        [DisplayName("Event Link")]
        [MaxLength(300)]
        public string ExternalLink { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [DisplayName("Type")]
        public bool IsCommunityExperience { get; set; }

        public bool IsFavorited { get; set; }

        [DisplayName("Is this a streaming event?")]
        public bool IsStreaming { get; set; }

        [DisplayName("Embed stream?")]
        public bool IsStreamingEmbed { get; set; }

        [Required]
        public bool IsValid { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public int? ParentEventId { get; set; }

        [DisplayName("For Program")]
        public int? ProgramId { get; set; }

        [Required]
        public int RelatedBranchId { get; set; }

        [Required]
        public int RelatedSystemId { get; set; }

        public int? RelatedTriggerId { get; set; }
        public int SiteId { get; set; }

        [DisplayName("Start")]
        [Required]
        public DateTime StartDate { get; set; }

        [DisplayName("Access to stream ends")]
        public DateTime? StreamingAccessEnds { get; set; }

        [MaxLength(2000)]
        [DisplayName("Stream link/embed code")]
        public string StreamingLinkData { get; set; }
    }
}