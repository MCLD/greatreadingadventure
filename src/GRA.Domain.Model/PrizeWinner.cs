using System;

namespace GRA.Domain.Model
{
    public class PrizeWinner : Abstract.BaseDomainEntity
    {
        public string AvailableAtBranch { get; set; }
        public string AvailableAtBranchUrl { get; set; }
        public string AvailableAtSystem { get; set; }
        public int? DrawingId { get; set; }
        public int? MailId { get; set; }
        public string PrizeName { get; set; }
        public string PrizeRedemptionInstructions { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public int? RedeemedBy { get; set; }
        public int? RedeemedByBranch { get; set; }
        public int? RedeemedBySystem { get; set; }
        public int SiteId { get; set; }
        public string StaffNotes { get; set; }
        public int? TriggerId { get; set; }
        public string UserFirstName { get; set; }
        public int UserId { get; set; }
        public string UserLastName { get; set; }
        public string UserUsername { get; set; }
        public int? VendorCodeId { get; set; }
    }
}
