using System.Collections.Generic;

namespace GRA.Domain.Model.Filters
{
    public class GroupFilter : BaseFilter
    {
        public ICollection<int> GroupTypeIds { get; set; }

        public GroupFilter(int? page = null) : base(page) { }
    }
}
