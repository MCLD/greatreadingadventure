using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class JobDetailsReceivePackingSlip
    {
        public long PackingSlipNumber { get; set; }
        public IEnumerable<int> DamagedItems { get; set; }
        public IEnumerable<int> MissingItems { get; set; }
    }
}
