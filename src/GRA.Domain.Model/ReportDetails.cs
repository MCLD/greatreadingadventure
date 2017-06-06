using System;

namespace GRA.Domain.Model
{
    public class ReportDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type ReportType { get; set; }
    }
}
