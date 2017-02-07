using System;

namespace GRA.Domain.Model
{
    public class VendorCode : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int VendorCodeTypeId { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public DateTime DateUsed { get; set; }
        public int UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }

        public string VendorCodeTypeDescription { get; set; }
    }
}
