namespace GRA.Domain.Model.Filters
{
    public class NewsFilter : BaseFilter
    {
        public bool DefaultCategory { get; set; }

        public NewsFilter(int? page = null, int take = 15) : base(page, take) { }
    }
}
