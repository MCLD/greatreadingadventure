namespace GRA.Domain.Model
{
    public class StatusSummary
    {
        public string BranchName { get; set; }

        public int RegisteredUsers { get; set; }
        public long PointsEarned { get; set; }
        public long CompletedChallenges { get; set; }
        public long BadgesEarned { get; set; }
        public int? DaysUntilEnd { get; set; }
        public long Achievers { get; set; }
    }
}
