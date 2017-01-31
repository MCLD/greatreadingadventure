using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class Filter
    {
        public ICollection<int?> SystemIds { get; set; }
        public ICollection<int?> BranchIds { get; set; }
        public ICollection<int?> ProgramIds { get; set; }
        public ICollection<int?> LocationIds { get; set; }
    }
}
