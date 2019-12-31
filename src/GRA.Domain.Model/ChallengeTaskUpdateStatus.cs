namespace GRA.Domain.Model
{
    public class ChallengeTaskUpdateStatus
    {
        public ChallengeTask ChallengeTask { get; set; }
        public bool WasComplete { get; set; }
        public bool IsComplete { get; set; }
    }
}
