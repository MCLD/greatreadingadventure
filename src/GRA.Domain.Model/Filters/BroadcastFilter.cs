using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class BroadcastFilter : BaseFilter
    {
        public bool? Upcoming { get; set; }

        public BroadcastFilter(int? page = null) : base(page) { }
    }
}
