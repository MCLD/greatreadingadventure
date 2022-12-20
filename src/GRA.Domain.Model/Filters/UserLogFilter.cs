namespace GRA.Domain.Model.Filters
{
    public class UserLogFilter : BaseFilter
    {
        public UserLogFilter(int? page = null, int take = 12) : base(page, take)
        {
        }

        public bool? HasAttachment { get; set; }
        public bool? HasBadge { get; set; }
    }
}
