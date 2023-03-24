using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class MessageTemplateText
    {
        [MaxLength(2000)]
        public string Body { get; set; }

        public int LanguageId { get; set; }
        public int MessageTemplateId { get; set; }

        [MaxLength(500)]
        public string Subject { get; set; }
    }
}
