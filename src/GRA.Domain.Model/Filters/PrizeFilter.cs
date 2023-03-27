namespace GRA.Domain.Model.Filters
{
    public class PrizeFilter : BaseFilter
    {
        public PrizeFilter(int? page = null) : base(page)
        {
        }

        public bool IncludeDrawings { get; set; }
        public bool? IsRedeemed { get; set; }
    }
}
