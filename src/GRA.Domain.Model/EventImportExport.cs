using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model
{
    public class EventImportExport
    {
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public string Description { get; set; }
        public string SecretCode { get; set; }
        public int Points { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string BadgeFilePath { get; set; }
    }
}
