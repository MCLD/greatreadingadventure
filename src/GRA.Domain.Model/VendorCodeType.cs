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
        [MaxLength(255)]
        public string DonationOptionSubject { get; set; }
        [MaxLength(1250)]
        public string DonationOptionMail { get; set; }
        [MaxLength(255)]
        public string DonationSubject { get; set; }
        [MaxLength(1250)]
        public string DonationMail { get; set; }
        [MaxLength(255)]
        public string DonationMessage { get; set; }
        [MaxLength(255)]
        public string Url { get; set; }
    }
}
