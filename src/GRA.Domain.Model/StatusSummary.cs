using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GRA.Domain.Model
{
    public class StatusSummary
    {
        public int? SiteId { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }

        public string BranchName { get; set; }

        public int RegisteredUsers { get; set; }
        public int PointsEarned { get; set; }
        public Dictionary<string, int> ActivityEarnings { get; set; }
        public int CompletedChallenges { get; set; }
    }
}
