namespace GRA.Domain.Model
{
    public class VendorCodeStatus
    {
        public int All { get; set; }
        public int Arrived { get; set; }
        public int AssignedCodes { get; set; }
        public int Donated { get; set; }
        public int EmailAwardDownloadedInReport { get; set; }
        public int EmailAwardPendingDownload { get; set; }
        public int EmailAwardSelected { get; set; }
        public int EmailAwardSent { get; set; }
        public bool IsConfigured { get; set; }
        public int NoStatus { get; set; }
        public int Ordered { get; set; }
        public int ReassignedCodes { get; set; }
        public int Shipped { get; set; }

        public int TotalCodes
        { get { return AssignedCodes + ReassignedCodes + UnusedCodes; } }

        public int UnusedCodes { get; set; }
        public int VendorSelected { get; set; }

        public string Percent(int items, int total, string label)
        {
            return total == 0
                ? "\u00A0"
                : string.Format("{0:0.00}% {1}", 100.0 * items / total * 2, label);
        }
    }
}
