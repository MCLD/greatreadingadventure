using System;

namespace GRA.Domain.Model
{
    public class ReportRequestSummary
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RequestedByUserId { get; set; }
        public bool? Success { get; set; }
        public string Message { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Finished { get; set; }
    }
}
