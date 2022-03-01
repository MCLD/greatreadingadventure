using System;

namespace GRA.Domain.Model.Utility
{
    public class EmailReminderExport
    {
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public int? LanguageId { get; set; }
        public string SignUpSource { get; set; }
    }
}
