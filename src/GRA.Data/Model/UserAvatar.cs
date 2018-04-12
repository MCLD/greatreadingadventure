using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserAvatar
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int AvatarElementId { get; set; }
        public AvatarElement AvatarElement { get; set; }
    }
}
