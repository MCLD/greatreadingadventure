using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class StatusSummary
    {
        public int? SiteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }

        public int RegisteredUsers { get; set; }
        public int PointsEarned { get; set; }
        public Dictionary<string, int> ActivityEarnings { get; set; }
        public int CompletedChallenges { get; set; }
    }
}
