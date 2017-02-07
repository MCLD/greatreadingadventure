using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCodeType : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        public Site Site { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
