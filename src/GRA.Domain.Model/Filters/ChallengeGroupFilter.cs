using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class ChallengeGroupFilter : BaseFilter
    {
        public ICollection<int> ChallengeGroupIds { get; set; }
        public bool? ActiveGroups { get; set; }

        public ChallengeGroupFilter(int? page = null, int? take = null) : base(page) { }
    }
}
