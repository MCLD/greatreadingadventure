using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCodeType : Abstract.BaseDomainEntity
    {
        [Display(Name = "Award prize on received packing slip",
            Description = "Automatically show a prize as 'awarded' in the participant's profile upon reception of the packing slip")]
        public bool AwardPrizeOnPackingSlip { get; set; }

        [Display(Name = "Award prize on ship date",
            Description = "Automatically show a prize as 'awarded' in the participant's profile upon ship date import")]
        public bool AwardPrizeOnShipDate { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Description = "Description of the vendor code type (e.g. 'Free book code')")]
        public string Description { get; set; }

        // Must be set if DonationSubject is set
        [Display(Name = "Donation message",
            Description = "Message to send to an achiever offering them an award choice")]
        public int? DonationMessageTemplateId { get; set; }

        // Must be set if DonationSubject is set
        [Display(Name = "Donation pop-up message",
            Description = "Pop-up message to show an achiever who chose to donate their prize")]
        public int? DonationSegmentId { get; set; }

        // Must be set if EmailAwardSubject is set
        [Display(Name = "Email award message",
            Description = "Message to send an achiever who chose an emailed prize")]
        public int? EmailAwardMessageTemplateId { get; set; }

        // Must be set if EmailAwardSubject is set
        [Display(Name = "Email award pop-up message",
            Description = "Pop-up message to show an achiever who chose an emailed prize")]
        public int? EmailAwardSegmentId { get; set; }

        [Display(Name = "Expiration date",
            Description = "Absolute expiration date of this vendor code after which it shouldn't be assigned/used (optional)")]
        public DateTime? ExpirationDate { get; set; }

        [Required]
        [Display(Name = "Message with code",
            Description = "Message to send an achiever with a vendor code award, available tokens: {{Code}} and {{Link}}")]
        public int MessageTemplateId { get; set; }

        // Must be set if OptionSubject is set
        [Display(Name = "Option message",
            Description = "Mail to send to an achiever offering them an award choice")]
        public int? OptionMessageTemplateId { get; set; }

        [Display(Name = "Ready for pickup email template",
            Description = "Email template to send participant when their vendor code item is ready for pick up")]
        public int? ReadyForPickupEmailTemplateId { get; set; }

        public int SiteId { get; set; }

        [MaxLength(255)]
        [Display(Description = "URL to redeem a vendor-code based prize, {{BranchId}} will be replaced with the participant's branch id, {{Code}} will be replaced with the vendor code")]
        public string Url { get; set; }
    }
}
