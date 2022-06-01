namespace GRA.Domain.Model
{
    public class JobDetailsSendBulkEmails
    {
        public int EmailTemplateId { get; set; }
        public string MailingList { get; set; }
        public int? TestLanguageId { get; set; }
        public string To { get; set; }
        public string UnsubscribeBase { get; set; }
    }
}
