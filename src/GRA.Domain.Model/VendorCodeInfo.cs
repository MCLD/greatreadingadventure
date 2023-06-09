using System;

namespace GRA.Domain.Model
{
    public class VendorCodeInfo
    {
        /// <summary>
        /// Possibility exists for customer to donate
        /// </summary>
        public bool CanDonate { get; set; }

        public bool CanEmailAward { get; set; }
        public string EmailAwardInstructions { get; set; }

        /// <summary>
        /// Item was received damaged
        /// </summary>
        public bool IsDamaged { get; set; }

        /// <summary>
        /// Participant chose do donate in our software
        /// </summary>
        public bool? IsDonated { get; set; }

        /// <summary>
        /// Donated through our vendor
        /// </summary>
        public bool IsDonationLocked { get; set; }

        public bool? IsEmailAwarded { get; set; }

        /// <summary>
        /// Item was present on packing slip but missing from package
        /// </summary>
        public bool IsMissing { get; set; }

        public bool NeedsToAnswerVendorCodeQuestion { get; set; }
        public VendorOrderStatus? OrderStatus { get; set; }
        public string PackingSlipLink { get; set; }
        public DateTime? PickupDate { get; set; }
        public string ReassignedByLink { get; set; }
        public string ReassignedByUser { get; set; }
        public VendorCode VendorCode { get; set; }
        public string VendorCodeDisplay { get; set; }
        public string VendorCodeMessage { get; set; }
        public string VendorCodePackingSlip { get; set; }
        public string VendorCodeUrl { get; set; }
    }
}
