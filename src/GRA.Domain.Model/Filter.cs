using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class Filter
    {
        public int? SiteId { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public ICollection<int> UserIds { get; set; }
        public ICollection<int> SystemIds { get; set; }
        public ICollection<int> BranchIds { get; set; }
        public ICollection<int?> ProgramIds { get; set; }
        public ICollection<int?> LocationIds { get; set; }
        public ICollection<int> BadgeIds { get; set; }
        public ICollection<int> ChallengeIds { get; set; }
        public string Search { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Filter(int? page = null)
        {
            this.Take = 15;
            if (page.HasValue)
            {
                this.Skip = this.Take * (page - 1);
            }
        }
    }
}
