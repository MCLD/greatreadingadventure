using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class EventFilter : BaseFilter
    {
        public int? EventType { get; set; }
        public int? SpatialDistanceHeaderId { get; set; }
        public SortEventsBy SortBy { get; set; }

        public EventFilter(int? page = null) : base(page)
        {
        }
    }
}
