using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class EmailSubscriptionAuditLog : Abstract.BaseDbEntity
    {
        public int UserId { get; set; }
        public bool Subscribed { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }

        public bool TokenUsed { get; set; }
    }
}
