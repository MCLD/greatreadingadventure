﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class VendorCode : Abstract.BaseDbEntity
    {
        public DateTime? ArrivalDate { get; set; }
        public int? AssociatedUserId { get; set; }
        public int? BranchId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        public DateTime DateUsed { get; set; }

        [MaxLength(255)]
        public string Details { get; set; }

        [MaxLength(255)]
        public string EmailAwardAddress { get; set; }

        public DateTime? EmailAwardReported { get; set; }
        public DateTime? EmailAwardSent { get; set; }
        public DateTime? EmailSentAt { get; set; }
        public bool IsAssigned { get; set; }
        public bool? IsDamaged { get; set; }
        public bool? IsDonated { get; set; }
        public bool IsDonationLocked { get; set; }
        public bool? IsEmailAward { get; set; }
        public bool? IsMissing { get; set; }

        [Required]
        public bool IsUsed { get; set; }

        public DateTime? OrderDate { get; set; }

        [MaxLength(255)]
        public string PackingSlip { get; set; }

        [MaxLength(255)]
        public string ReasonForReassignment { get; set; }

        public DateTime? ReassignedAt { get; set; }
        public int? ReassignedByUserId { get; set; }

        /// <summary>
        /// If an import has an order date after a ship date it's detected as a reshipment, this
        /// is the date and time that the reshipment was detected.
        /// </summary>
        public DateTime? ReshipmentDetectedDate { get; set; }

        /// <summary>
        /// If a reshipment was detected, this was the prior packing slip.
        /// </summary>
        [MaxLength(255)]
        public string ReshipmentPriorPackingSlip { get; set; }

        public DateTime? ShipDate { get; set; }

        [Required]
        public int SiteId { get; set; }

        [MaxLength(512)]
        public string TrackingNumber { get; set; }

        [ConcurrencyCheck]
        public int? UserId { get; set; }

        public VendorCodeType VendorCodeType { get; set; }

        [Required]
        public int VendorCodeTypeId { get; set; }
    }
}
