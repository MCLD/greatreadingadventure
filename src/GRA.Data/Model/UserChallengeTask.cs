using System;

namespace GRA.Data.Model
{
    public class UserChallengeTask
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChallengeTaskId { get; set; }
        public ChallengeTask ChallengeTask { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UserLogId { get; set; }
        public int? BookId { get; set; }
    }
}