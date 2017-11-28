using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class ChallengeFilter : BaseFilter
    {
        public bool? Favorites { get; set; }

        public ChallengeFilter(int? page = null) : base(page) { }
    }
}
