using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCodeType : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        public Site Site { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [MaxLength(1250)]
        public string Mail { get; set; }

        [Required]
        [MaxLength(255)]
        public string MailSubject { get; set; }

        [MaxLength(255)]
        public string OptionSubject { get; set; }

        [MaxLength(1250)]
        public string OptionMail { get; set; }

        [MaxLength(255)]
        public string DonationSubject { get; set; }

        [MaxLength(1250)]
        public string DonationMail { get; set; }

        [MaxLength(255)]
        public string DonationMessage { get; set; }

        [MaxLength(255)]
        public string EmailAwardSubject { get; set; }

        [MaxLength(1250)]
        public string EmailAwardMail { get; set; }

        [MaxLength(255)]
        public string EmailAwardMessage { get; set; }

        [MaxLength(255)]
#pragma warning disable CA1056 // URI-like properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // URI-like properties should not be strings

        public DateTime? ExpirationDate { get; set; }
        public bool AwardPrizeOnShipDate { get; set; }
        public bool AwardPrizeOnPackingSlip { get; set; }
        public int? ReadyForPickupEmailTemplateId { get; set; }

    }
}
