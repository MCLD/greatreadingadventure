namespace GRA.Domain.Model
{
    public class EmailSubscriptionAuditLog : Abstract.BaseDomainEntity
    {
        public int UserId { get; set; }
        public bool Subscribed { get; set; }
        public string CreatedByName { get; set; }
        public bool TokenUsed { get; set; }
    }
}
