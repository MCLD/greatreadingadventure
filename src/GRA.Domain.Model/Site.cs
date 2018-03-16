using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Page Title")]
        [Required]
        [MaxLength(255)]
        public string PageTitle { get; set; }
        [MaxLength(255)]
        public string Domain { get; set; }
        public bool IsDefault { get; set; }
        public string Footer { get; set; }

        [DisplayName("Before RegistrationPage")]
        public int? BeforeRegistrationPage { get; set; }
        [DisplayName("Registration Opens")]
        public DateTime? RegistrationOpens { get; set; }
        [DisplayName("Registration Open Page")]
        public int? RegistrationOpenPage { get; set; }
        [DisplayName("Program Starts")]
        public DateTime? ProgramStarts { get; set; }
        [DisplayName("Program Starts Page")]
        public int? ProgramOpenPage { get; set; }
        [DisplayName("Program Ends")]
        public DateTime? ProgramEnds { get; set; }
        [DisplayName("Program Ended Page")]
        public int? ProgramEndedPage { get; set; }
        [DisplayName("Access Closed")]
        public DateTime? AccessClosed { get; set; }
        [DisplayName("Access Closed Page")]
        public int? AccessClosedPage { get; set; }

        [DisplayName("Outgoing Mail Host")]
        public string OutgoingMailHost { get; set; }
        [DisplayName("Outgoing Mail Port")]
        public int? OutgoingMailPort { get; set; }
        [DisplayName("Outgoing Mail Login")]
        public string OutgoingMailLogin { get; set; }
        [DisplayName("Outgoing Mail Password")]
        public string OutgoingMailPassword { get; set; }

        [DisplayName("External Event List Url")]
        public string ExternalEventListUrl { get; set; }
        [DisplayName("Google Analytics Tracking Id")]
        public string GoogleAnalyticsTrackingId { get; set; }
        [DisplayName("Collect Preregistration Emails")]
        public bool CollectPreregistrationEmails { get; set; }
        [DisplayName("Meta Description")]
        [MaxLength(150)]
        public string MetaDescription { get; set; }

        [DisplayName("Require Postal Code")]
        public bool RequirePostalCode { get; set; }
        [DisplayName("Single Page Sign Up")]
        public bool SinglePageSignUp { get; set; }

        [DisplayName("Max Points Per Challenge Task")]
        public int? MaxPointsPerChallengeTask { get; set; }

        [DisplayName("From Email Name")]
        public string FromEmailName { get; set; }
        [DisplayName("From Email Address")]
        public string FromEmailAddress { get; set; }

        [DisplayName("Site Logo Url")]
        [MaxLength(100)]
        public string SiteLogoUrl { get; set; }
        [DisplayName("Facebook App Id")]
        [MaxLength(100)]
        public string FacebookAppId { get; set; }
        [DisplayName("Facebook Image Url")]
        [MaxLength(100)]
        public string FacebookImageUrl { get; set; }
        [DisplayName("Twitter Large Card")]
        public bool? TwitterLargeCard { get; set; }
        [DisplayName("Twitter Card ImageUrl")]
        [MaxLength(100)]
        public string TwitterCardImageUrl { get; set; }
        [DisplayName("Twitter Username")]
        [MaxLength(15)]
        public string TwitterUsername { get; set; }
        [DisplayName("Is Https Forced")]
        public bool IsHttpsForced { get; set; }
        public ICollection<SiteSetting> Settings { get; set; }
    }
}
