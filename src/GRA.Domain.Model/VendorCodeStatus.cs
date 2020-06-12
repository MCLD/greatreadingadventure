namespace GRA.Domain.Model
{
    public class VendorCodeStatus
    {
        public int AssignedCodes { get; set; }
        public int Donated { get; set; }
        public int EmailAwardDownloadedInReport { get; set; }
        public int EmailAwardPendingDownload { get; set; }
        public int EmailAwardSelected { get; set; }
        public int EmailAwardSent { get; set; }
        public int NoStatus { get; set; }
        public int Ordered { get; set; }
        public int Shipped { get; set; }
        public int TotalCodes { get { return AssignedCodes + UnusedCodes; } }
        public int UnusedCodes { get; set; }
        public int VendorSelected { get; set; }
        public string Percent(int items, int total, string label)
        {
            return total == 0
                ? "\u00A0"
                : string.Format("{0:0.00}% {1}", items * 100.0 / total, label);
        }
    }
}
