using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.CommandLine.DataGenerator
{
    internal class GeneratedActivity
    {
        public ActivityType ActivityType { get; set; }
        public Domain.Model.User User { get; set; }
        public int ActivityAmount { get; set; }
        public string SecretCode { get; set; }
        public int ChallengeId { get; set; }
        public ICollection<ChallengeTask> ChallengeTasks { get; set; }
    }
}
