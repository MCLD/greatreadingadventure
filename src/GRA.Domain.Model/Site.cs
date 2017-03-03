using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Site : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Path { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string PageTitle { get; set; }
        [MaxLength(255)]
        public string Domain { get; set; }
        public bool IsDefault { get; set; }
        public string Footer { get; set; }

        public int? BeforeRegistrationPage { get; set; }
        public DateTime? RegistrationOpens { get; set; }
        public int? RegistrationOpenPage { get; set; }
        public DateTime? ProgramStarts { get; set; }
        public int? ProgramOpenPage { get; set; }
        public DateTime? ProgramEnds { get; set; }
        public int? ProgramEndedPage { get; set; }
        public DateTime? AccessClosed { get; set; }
        public int? AccessClosedPage { get; set; }

        public string OutgoingMailHost { get; set; }
        public int? OutgoingMailPort { get; set; }
        public string OutgoingMailLogin { get; set; }
        public string OutgoingMailPassword { get; set; }

        public string ExternalEventListUrl { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public bool CollectPreregistrationEmails { get; set; }
        [MaxLength(150)]
        public string MetaDescription { get; set; }

        public bool RequirePostalCode { get; set; }
        public bool SinglePageSignUp { get; set; }

        public int? MaxPointsPerChallengeTask { get; set; }

        public string FromEmailName { get; set; }
        public string FromEmailAddress { get; set; }

        public bool UseDynamicAvatars { get; set; }
        [MaxLength(100)]
        public string SiteLogoUrl { get; set; }
        [MaxLength(100)]
        public string FacebookAppId { get; set; }
        [MaxLength(100)]
        public string FacebookImageUrl { get; set; }
        public bool? TwitterLargeCard { get; set; }
        [MaxLength(100)]
        public string TwitterCardImageUrl { get; set; }
        [MaxLength(15)]
        public string TwitterUsername { get; set; }
    }
}
