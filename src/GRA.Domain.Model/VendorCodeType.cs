using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCodeType : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [MaxLength(1250)]
        public string Mail { get; set; }

        [Required]
        [MaxLength(255)]
        public string MailSubject { get; set; }

        // Setting this will require the user to select what they want to do with the code
        // instead of automatically showing it to them.
        [MaxLength(255)]
        public string OptionSubject { get; set; }

        // Must be set if OptionSubject is set
        [MaxLength(1250)]
        public string OptionMail { get; set; }

        // Setting this enables the donation option for vendor codes
        [MaxLength(255)]
        public string DonationSubject { get; set; }

        // Must be set if DonationSubject is set
        [MaxLength(1250)]
        public string DonationMail { get; set; }

        // Must be set if DonationSubject is set
        [MaxLength(255)]
        public string DonationMessage { get; set; }

        // Setting this enables the email award option for vendor codes
        [MaxLength(255)]
        public string EmailAwardSubject { get; set; }

        // Must be set if EmailAwardSubject is set
        [MaxLength(1250)]
        public string EmailAwardMail { get; set; }

        // Must be set if EmailAwardSubject is set
        [MaxLength(255)]
        public string EmailAwardMessage { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}
