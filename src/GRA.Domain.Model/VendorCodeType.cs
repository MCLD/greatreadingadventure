﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCodeType : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Description = "Description of the vendor code type (e.g. 'Free book code')")]
        public string Description { get; set; }

        [Required]
        [MaxLength(1250)]
        [Display(Description = "Text of the mail to send an achiever with a vendor code award, {Code} will be replaced with their code")]
        public string Mail { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Mail subject", Description = "Subject of the mail to send an achiever with a vendor code award")]
        public string MailSubject { get; set; }

        // Setting this will require the user to select what they want to do with the code
        // instead of automatically showing it to them.
        [MaxLength(255)]
        [Display(Name = "Option subject",
            Description = "Subject of the mail to send an achiever offering them an award choice")]
        public string OptionSubject { get; set; }

        // Must be set if OptionSubject is set
        [Display(Name = "Option mail",
            Description = "Mail to send to an achiever offering them an award choice")]
        [MaxLength(1250)]
        public string OptionMail { get; set; }

        // Setting this enables the donation option for vendor codes
        [Display(Name = "Donation subject",
            Description = "Subject of the mail to send an achiever who chose to donate their prize")]
        [MaxLength(255)]
        public string DonationSubject { get; set; }

        // Must be set if DonationSubject is set
        [Display(Name = "Donation mail",
            Description = "Mail to send an achiever who chose to donate their prize")]
        [MaxLength(1250)]
        public string DonationMail { get; set; }

        // Must be set if DonationSubject is set
        [Display(Name = "Donation message",
            Description = "Pop-up message to show an achiever who chose to donate their prize")]
        [MaxLength(255)]
        public string DonationMessage { get; set; }

        // Setting this enables the email award option for vendor codes
        [Display(Name = "Email award subject",
            Description = "Subject of the mail to send an achiever who chose an emailed prize")]
        [MaxLength(255)]
        public string EmailAwardSubject { get; set; }

        // Must be set if EmailAwardSubject is set
        [Display(Name = "Email award mail",
            Description = "Mail to send an achiever who chose an emailed prize")]
        [MaxLength(1250)]
        public string EmailAwardMail { get; set; }

        // Must be set if EmailAwardSubject is set
        [Display(Name = "Email award message",
            Description = "Pop-up message to show an achiever who chose an emailed prize")]
        [MaxLength(255)]
        public string EmailAwardMessage { get; set; }

        [MaxLength(255)]
        [Display(Description = "URL to redeem a vendor-code based prize, {Code} will be replaced with the vendor code")]
#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        [Display(Name = "Expiration date",
            Description = "Absolute expiration date of this vendor code after which it shouldn't be assigned/used (optional)")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Award prize on ship date",
            Description = "Automatically show a prize as 'awarded' in the participant's profile upon ship date import")]
        public bool AwardPrizeOnShipDate { get; set; }

        [Display(Name = "Award prize on received packing slip",
            Description = "Automatically show a prize as 'awarded' in the participant's profile upon reception of the packing slip")]
        public bool AwardPrizeOnPackingSlip { get; set; }
    }
}
