namespace GRA.Domain.Model.Filters
{
    public class EventFilter : BaseFilter
    {
        public int? EventType { get; set; }
        public int? SpatialDistanceHeaderId { get; set; }
        public SortEventsBy SortBy { get; set; }
        public bool? OnlyFavorites { get; set; }
        public int? FavoritesUserId { get; set; }
        public bool? HideFavorites { get; set; }

        public EventFilter(int? page = null) : base(page)
        {
        }
    }
}
