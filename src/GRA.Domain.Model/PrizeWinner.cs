using System;

namespace GRA.Domain.Model
{
    public class PrizeWinner : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int? DrawingId { get; set; }
        public int? TriggerId { get; set; }
        public int UserId { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public int? RedeemedBy { get; set; }
        public int? MailId { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserUsername { get; set; }
        public string PrizeName { get; set; }
        public string PrizeRedemptionInstructions { get; set; }
    }
}
