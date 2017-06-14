using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class StoredReportSet
    {
        public ICollection<StoredReport> Reports { get; set; }
    }
}
