using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class VendorCode : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int VendorCodeTypeId { get; set; }
        public string Code { get; set; }
        public bool IsUsed { get; set; }
        public DateTime DateUsed { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public string VendorCodeTypeDescription { get; set; }
        public bool? IsDonated { get; set; }
        public bool CanBeDonated { get; set; }
        [MaxLength(255)]
        public string Url { get; set; }
        [MaxLength(255)]
        public string Details { get; set; }
    }
}
