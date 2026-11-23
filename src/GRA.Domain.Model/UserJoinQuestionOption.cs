using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class UserJoinQuestionOption
    {
        public User User { get; set; }
        [Required]
        public int UserId { get; set; }
        public JoinQuestionOption JoinQuestionOption { get; set; }
        [Required]
        public int JoinQuestionOptionId { get; set; }
    }
}
