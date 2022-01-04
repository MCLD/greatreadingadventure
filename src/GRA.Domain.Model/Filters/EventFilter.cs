namespace GRA.Domain.Model.Filters
{
    public class EventFilter : BaseFilter
    {
        public EventFilter(int? page = null) : base(page)
        {
        }

        public bool ByStreamingStartDesc { get; set; }
        public int? CurrentUserId { get; set; }
        public int? EventType { get; set; }
        public bool? Favorites { get; set; }
        public bool? IsAttended { get; set; }
        public bool? IsStreamingNow { get; set; }
        public SortEventsBy SortBy { get; set; }
        public int? SpatialDistanceHeaderId { get; set; }
    }
}
