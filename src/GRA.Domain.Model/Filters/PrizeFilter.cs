namespace GRA.Domain.Model.Filters
{
    public class PrizeFilter : BaseFilter
    {
        public PrizeFilter() : base(null)
        {
        }

        public PrizeFilter(int page) : base(page)
        {
        }

        public bool IncludeDrawings { get; set; }
        public bool? IsRedeemed { get; set; }
    }
}
