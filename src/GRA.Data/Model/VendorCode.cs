using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCode : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public int VendorCodeTypeId { get; set; }
        public VendorCodeType VendorCodeType { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public bool IsUsed { get; set; }
        public DateTime DateUsed { get; set; }
        [ConcurrencyCheck]
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
    }
}
