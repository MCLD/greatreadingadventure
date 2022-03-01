using System;
using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class EmailListExport
    {
        public IEnumerable<EmailReminderExport> Addresses { get; set; }
        public DateTime ExportedAt { get; set; }
        public string ExportedBy { get; set; }
        public string Source { get; set; }
        public int Version { get; set; }
    }
}
