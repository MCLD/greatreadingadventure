using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class PackingSlipSummary
    {
        public long PackingSlipNumber { get; set; }
        public string SubmitText { get; set; }
        public ICollection<VendorCode> VendorCodes { get; set; }
        public VendorCodePackingSlip VendorCodePackingSlip { get; set; }
    }
}
