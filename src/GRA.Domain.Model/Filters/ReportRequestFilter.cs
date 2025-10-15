namespace GRA.Domain.Model.Filters
{
    public class ReportRequestFilter : BaseFilter
    {
        public ReportRequestFilter(int? page) : base(page)
        {
        }

        public int? ReportId { get; set; }
        public int? RequestedByUserId { get; set; }
    }
}
