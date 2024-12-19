using System.Collections.Generic;

namespace GRA.Domain.Model.Filters
{
    public class TriggerFilter : BaseFilter
    {
        public bool? SecretCodesOnly { get; set; }
        public bool? LowPointActivated { get; set; }
        public ICollection<int> LowPointsIds { get; set; }
        public TriggerFilter(int? page = null) : base(page) { }
    }
}
