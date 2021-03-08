using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCodePackingSlip : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        public long PackingSlip { get; set; }

        public bool IsReceived { get; set; }
    }
}
