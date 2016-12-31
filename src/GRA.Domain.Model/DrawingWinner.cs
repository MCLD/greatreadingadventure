using System;

namespace GRA.Domain.Model
{
    public class DrawingWinner
    {
        public int DrawingId { get; set; }
        public int UserId { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public int? MailId { get; set; }

        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserUsername { get; set; }
        public string DrawingName { get; set; }
        public string DrawingRedemptionInstructions { get; set; }
    }
}
