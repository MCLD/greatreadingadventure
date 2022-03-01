using System;
using System.Collections.Generic;

namespace GRA.Domain.Model.Utility
{
    public class EmailListImport
    {
        public IEnumerable<EmailReminder> Addresses { get; set; }
        public DateTime ExportedAt { get; set; }
        public string ExportedBy { get; set; }
        public string Source { get; set; }
        public int Version { get; set; }
    }
}
