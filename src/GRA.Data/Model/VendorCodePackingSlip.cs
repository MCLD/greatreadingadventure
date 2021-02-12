using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCodePackingSlip : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public long PackingSlip { get; set; }

        public bool IsReceived { get; set; }
    }
}
