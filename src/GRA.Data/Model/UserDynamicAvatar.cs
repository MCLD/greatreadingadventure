using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserDynamicAvatar
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int DynamicAvatarElementId { get; set; }
        public DynamicAvatarElement DynamicAvatarElement { get; set; }
    }
}
