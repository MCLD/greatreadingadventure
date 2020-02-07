namespace GRA.Domain.Model.Filters
{
    public class ChallengeFilter : BaseFilter
    {
        public bool OnlyFavorites { get; set; }
        public int? FavoritesUserId { get; set; }
        public bool HideFavorites { get; set; }
        public int? GroupId { get; set; }

        public ChallengeFilter(int? page = null, int? take = null) : base(page) { }
    }
}
