using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Site : Abstract.BaseDbEntity
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

        public virtual ICollection<Challenge> Challenges { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<System> Systems { get; set; }

        public string ExternalEventListUrl { get; set; }
        public string GoogleAnalyticsTrackingId { get; set; }
        public bool CollectPreregistrationEmails { get; set; }
        [MaxLength(150)]
        public string MetaDescription { get; set; }

        public bool RequirePostalCode { get; set; }
        public bool SinglePageSignUp { get; set; }
    }
}
