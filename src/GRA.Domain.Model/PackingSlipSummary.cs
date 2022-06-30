using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class PackingSlipSummary
    {
        public bool CanBeReceived { get; set; }
        public bool CanViewDetails { get; set; }
        public IEnumerable<int> DamagedItems { get; set; }
        public IEnumerable<int> MissingItems { get; set; }
        public long PackingSlipNumber { get; set; }
        public string ReceivedBy { get; set; }
        public string SubmitText { get; set; }
        public IEnumerable<string> TrackingNumbers { get; set; }
        public VendorCodePackingSlip VendorCodePackingSlip { get; set; }
        public ICollection<VendorCode> VendorCodes { get; set; }
    }
}
