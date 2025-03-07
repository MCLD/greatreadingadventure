using System;

namespace GRA.Domain.Model
{
    public class ReportDetails
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Type ReportType { get; set; }
        public string RequiresPermission { get; set; }
    }
}
