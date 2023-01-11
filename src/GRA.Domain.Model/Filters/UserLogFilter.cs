namespace GRA.Domain.Model.Filters
{
    public class UserLogFilter : BaseFilter
    {
        public UserLogFilter(int? page) : this(page, 12)
        {
        }

        public UserLogFilter(int take) : this(null, take)
        {
        }

        public UserLogFilter(int? page, int take) : base(page, take)
        {
        }

        public bool? HasAttachment { get; set; }
        public bool? HasBadge { get; set; }
    }
}
