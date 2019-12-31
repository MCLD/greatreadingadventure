namespace GRA.Domain.Model.Filters
{
    public class DailyImageFilter : BaseFilter
    {
        public int DailyLiteracyTipId { get; set; }

        public DailyImageFilter(int? page = null) : base(page) { }
    }
}
