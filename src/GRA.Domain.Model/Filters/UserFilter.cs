
namespace GRA.Domain.Model.Filters
{
    public class UserFilter : BaseFilter
    {
        public SortUsersBy SortBy { get; set; }
        public bool OrderDescending { get; set; }
        public bool CanAddToHousehold { get; set; }

        public bool? IsSubscribed { get; set; }

        public UserFilter(int? page = null) : base(page)
        {
            SortBy = SortUsersBy.LastName;
        }
    }
}
