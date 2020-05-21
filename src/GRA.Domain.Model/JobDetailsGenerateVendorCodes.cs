using System;

namespace GRA.Domain.Model
{
    [Serializable]
    public class JobDetailsGenerateVendorCodes
    {
        public int VendorCodeTypeId { get; set; }

        public int NumberOfCodes { get; set; }

        public int CodeLength { get; set; }
    }
}
