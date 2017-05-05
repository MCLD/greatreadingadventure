using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Model
{
    public class TriggerRequirement
    {
        public int? BadgeId { get; set; }
        public int? ChallengeId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconDescription { get; set; }
        public string BadgePath { get; set; }
    }
}
