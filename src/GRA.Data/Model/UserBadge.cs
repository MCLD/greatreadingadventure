using System;

namespace GRA.Data.Model
{
    public class UserBadge
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int BadgeId { get; set; }
        public Badge Badge { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
