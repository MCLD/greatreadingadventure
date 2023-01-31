using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PrizeWinner : Abstract.BaseDbEntity
    {
        public Drawing Drawing { get; set; }

        public int? DrawingId { get; set; }

        public int? MailId { get; set; }

        public DateTime? RedeemedAt { get; set; }

        public int? RedeemedBy { get; set; }

        public int? RedeemedByBranch { get; set; }

        public int? RedeemedBySystem { get; set; }

        [Required]
        public int SiteId { get; set; }
        [MaxLength(255)]
        public string StaffNotes { get; set; }

        public Trigger Trigger { get; set; }
        public int? TriggerId { get; set; }
        public User User { get; set; }

        [Required]
        public int UserId { get; set; }
        public int? VendorCodeId { get; set; }
    }
}
