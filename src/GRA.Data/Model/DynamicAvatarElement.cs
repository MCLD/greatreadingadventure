using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarElement : Abstract.BaseDbEntity
    {
        [Required]
        public int DynamicAvatarItemId { get; set; }
        public DynamicAvatarItem DynamicAvatarItem { get; set; }

        public int? DynamicAvatarColorId { get; set; }
        public DynamicAvatarColor DynamicAvatarColor { get; set; }

        [MaxLength(255)]
        [Required]
        public string Filename { get; set; }
    }
}
