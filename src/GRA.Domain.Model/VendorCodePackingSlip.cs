using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCodePackingSlip : Abstract.BaseDomainEntity
    {
        public bool IsReceived { get; set; }

        [Required]
        public string PackingSlip { get; set; }

        public int SiteId { get; set; }
    }
}
