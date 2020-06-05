namespace GRA.Domain.Model.Filters
{
    public class EmailReminderFilter : BaseFilter
    {
        public string MailingList { get; set; }
        public bool HasReceived { get; set; }
    }
}
