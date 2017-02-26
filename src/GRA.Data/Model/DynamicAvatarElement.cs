using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarElement : Abstract.BaseDbEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int Position { get; set; }
    }
}
