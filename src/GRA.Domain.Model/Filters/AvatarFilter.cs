using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class AvatarFilter : BaseFilter
    {
        public bool? Unlockable { get; set; }
        public ICollection<int> ItemIds { get; set; }
        public int? LayerId { get; set; }

        public AvatarFilter(int? page = null, int take = 10) : base(page, take) { }
    }
}
