using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GRA.Domain.Model
{
    public class PackingSlipSummary
    {
        public bool CanBeReceived { get; set; }
        public bool CanViewDetails { get; set; }
        public IEnumerable<int> DamagedItems { get; set; }
        public IEnumerable<int> MissingItems { get; set; }
        public string PackingSlipNumber { get; set; }
        public string ProgramInfo { get; set; }
        public string ReceivedBy { get; set; }
        public string SubmitText { get; set; }
        public IEnumerable<string> TrackingNumbers { get; set; }
        public VendorCodePackingSlip VendorCodePackingSlip { get; set; }
        public ICollection<VendorCode> VendorCodes { get; set; }

        public static string LastCommaFirst(string firstName, string lastName)
        {
            var sb = new StringBuilder(lastName);
            if (!string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(firstName))
            {
                sb.Append(", ");
            }
            sb.Append(firstName);
            return sb.ToString().ToUpper(CultureInfo.CurrentCulture);
        }
    }
}
