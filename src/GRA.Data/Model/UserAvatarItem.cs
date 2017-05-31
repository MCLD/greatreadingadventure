using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserAvatarItem
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int DynamicAvatarItemId { get; set; }
        public DynamicAvatarItem DynamicAvatarItem { get; set; }
    }
}
