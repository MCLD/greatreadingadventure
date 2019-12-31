namespace GRA.Domain.Model.Filters
{
    public class DrawingFilter : BaseFilter
    {
        public bool Archived { get; set; }

        public DrawingFilter(int? page = null) : base(page) { }
    }
}
