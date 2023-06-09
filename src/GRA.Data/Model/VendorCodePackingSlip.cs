using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCodePackingSlip : Abstract.BaseDbEntity
    {
        public bool IsReceived { get; set; }

        [Required]
        [MaxLength(255)]
        public string PackingSlip { get; set; }

        [Required]
        public int SiteId { get; set; }
    }
}
