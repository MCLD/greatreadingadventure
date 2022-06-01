using System;

namespace GRA.Domain.Model.Abstract
{
    public abstract class BaseImportExport
    {
        public DateTime ExportedAt { get; set; }
        public string ExportedBy { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
    }
}
