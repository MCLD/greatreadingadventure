using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCodeType : Abstract.BaseDbEntity
    {
        public bool AwardPrizeOnPackingSlip { get; set; }

        public bool AwardPrizeOnShipDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public int? DonationMessageTemplateId { get; set; }
        public int? DonationSegmentId { get; set; }
        public int? EmailAwardMessageTemplateId { get; set; }
        public int? EmailAwardSegmentId { get; set; }
        public DateTime? ExpirationDate { get; set; }

        [Required]
        public int MessageTemplateId { get; set; }

        public int? OptionMessageTemplateId { get; set; }

        public int? ReadyForPickupEmailTemplateId { get; set; }

        public Site Site { get; set; }

        [Required]
        public int SiteId { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }
    }
}
