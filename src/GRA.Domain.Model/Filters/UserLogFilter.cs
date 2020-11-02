using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class UserLogFilter : BaseFilter
    {
        public bool? HasBadge { get; set; }

        public UserLogFilter(int? page = null, int take = 12) : base(page, take) { }
    }
}
