using System;

namespace GRA.Domain.Model.Report
{
    public class VendorCodeItemStatus
    {
        public DateTime? ArrivalDate { get; set; }
        public string Code { get; set; }
        public int? DeliveryBranchId { get; set; }
        public string FirstName { get; set; }
        public bool IsUserDeleted { get; set; }
        public string LastName { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderDetails { get; set; }
        public string PackingSlip { get; set; }
        public DateTime? ReassignedAt { get; set; }
        public int? ReassignedBy { get; set; }
        public string ReassignedFor { get; set; }
        public DateTime? ShipDate { get; set; }
        public string TrackingNumber { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
