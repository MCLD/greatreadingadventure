using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Sites
{
    public class SiteConfigurationViewModel
    {
        public int Id { get; set; }

        [DisplayName("Site Logo Url")]
        [MaxLength(100)]
        public string SiteLogoUrl { get; set; }
        [DisplayName("External Event List Url")]
        public string ExternalEventListUrl { get; set; }
        [DisplayName("Max Points Per Challenge Task")]
        public int? MaxPointsPerChallengeTask { get; set; }
        [DisplayName("Require Postal Code")]
        public bool RequirePostalCode { get; set; }
        [DisplayName("Single Page Sign Up")]
        public bool SinglePageSignUp { get; set; }
        [DisplayName("Google Analytics Tracking Id")]
        public string GoogleAnalyticsTrackingId { get; set; }
        [DisplayName("Is Https Forced")]
        public bool IsHttpsForced { get; set; }
        [DisplayName("From Email Name")]
        public string FromEmailName { get; set; }
        [DisplayName("From Email Address")]
        public string FromEmailAddress { get; set; }
        [DisplayName("Outgoing Mail Host")]
        public string OutgoingMailHost { get; set; }
        [DisplayName("Outgoing Mail Port")]
        public int? OutgoingMailPort { get; set; }
        [DisplayName("Outgoing Mail Login")]
        public string OutgoingMailLogin { get; set; }
        [DisplayName("Outgoing Mail Password")]
        public string OutgoingMailPassword { get; set; }
    }
}
