using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class StatusSummary
    {
        public int SiteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int RegisteredUsers { get; set; }
        public int PointsEarned { get; set; }
        public Dictionary<string, int> ActivityEarnings { get; set; }
        public int CompletedChallenges { get; set; }
    }
}
