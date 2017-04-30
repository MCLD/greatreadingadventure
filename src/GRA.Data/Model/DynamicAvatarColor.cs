using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarColor : Abstract.BaseDbEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }
        public DynamicAvatarLayer DynamicAvatarLayer { get; set; }

        [MaxLength(15)]
        [Required]
        public string Color { get; set; }
        public int SortOrder { get; set; }
    }
}
