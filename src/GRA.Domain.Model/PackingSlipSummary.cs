﻿using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class PackingSlipSummary
    {
        public long PackingSlipNumber { get; set; }
        public string SubmitText { get; set; }
        public ICollection<VendorCode> VendorCodes { get; set; }
        public VendorCodePackingSlip VendorCodePackingSlip { get; set; }
        public bool CanViewDetails { get; set; }
        public string ReceivedBy { get; set; }
        public bool CanReceivePackingSlips { get; set; }
        public IEnumerable<string> TrackingNumbers { get; set; }
    }
}
