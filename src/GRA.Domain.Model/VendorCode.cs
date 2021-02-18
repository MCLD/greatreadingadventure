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
        public DateTime? ArrivalDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public string VendorCodeTypeDescription { get; set; }
        public bool? IsDonated { get; set; }
        public bool? IsEmailAward { get; set; }

        [MaxLength(255)]
        public string EmailAwardAddress { get; set; }

        public DateTime? EmailAwardReported { get; set; }
        public DateTime? EmailAwardSent { get; set; }

        public bool CanBeDonated { get; set; }
        public bool CanBeEmailAward { get; set; }
        [MaxLength(255)]
#pragma warning disable CA1056 // Uri properties should not be strings
        public string Url { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings
        [MaxLength(255)]
        public string Details { get; set; }
        public string ParticipantName { get; set; }
        public int? BranchId { get; set; }
        public long PackingSlip { get; set; }
        [MaxLength(512)]
        public string TrackingNumber { get; set; }
    }
}
