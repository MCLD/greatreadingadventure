using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarElement : Abstract.BaseDbEntity
    {
        public AvatarColor AvatarColor { get; set; }

        public int? AvatarColorId { get; set; }

        public AvatarItem AvatarItem { get; set; }

        [Required]
        public int AvatarItemId { get; set; }
    }
}
