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
    }
}
