using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class Filter
    {
        public int? SiteId { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public ICollection<int?> SystemIds { get; set; }
        public ICollection<int?> BranchIds { get; set; }
        public ICollection<int?> ProgramIds { get; set; }
        public ICollection<int?> LocationIds { get; set; }
        public string Search { get; set; }
    }
}
