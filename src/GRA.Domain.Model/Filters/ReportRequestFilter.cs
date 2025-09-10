using System;

namespace GRA.Domain.Model.Filters
{
    public class ReportRequestFilter : BaseFilter
    {
        public int? ReportId { get; set; }
        public int? RequestedByUserId { get; set; }

        public ReportRequestFilter(int? page = null) : base (page) { }
    }
}
