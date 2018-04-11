using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AvatarColor : Abstract.BaseDbEntity
    {
        [Required]
        public int AvatarLayerId { get; set; }
        public AvatarLayer AvatarLayer { get; set; }

        [MaxLength(15)]
        [Required]
        public string Color { get; set; }
        public int SortOrder { get; set; }
    }
}
