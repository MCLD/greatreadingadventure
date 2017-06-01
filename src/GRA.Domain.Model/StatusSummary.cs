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
        public long PointsEarned { get; set; }
        public Dictionary<string, long> ActivityEarnings { get; set; }
        public long CompletedChallenges { get; set; }
        public long BadgesEarned { get; set; }
        public int? DaysUntilEnd { get; set; }
        public long Achievers { get; set; }
    }
}
