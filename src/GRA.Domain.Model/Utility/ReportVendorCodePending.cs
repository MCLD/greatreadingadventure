namespace GRA.Domain.Model.Utility
{
    public class ReportVendorCodePending
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public int OrderedNotShipped { get; set; }
        public int ShippedNotArrived { get; set; }
        public string SystemName { get; set; }
    }
}
