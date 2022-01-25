namespace GRA.Domain.Model.Filters
{
    public class ChallengeFilter : BaseFilter
    {
        public ChallengeFilter(int? page = null) : base(page)
        {
        }

        public ChallengeFilter(int page, int take) : base(page, take)
        {
        }

        public enum OrderingOption
        {
            Recent,
            Name,
            MostPopular
        }

        public bool? Favorites { get; set; }
        public int? FavoritesUserId { get; set; }
        public int? GroupId { get; set; }
        public bool? IsCompleted { get; set; }
        public OrderingOption Ordering { get; set; }
    }
}
