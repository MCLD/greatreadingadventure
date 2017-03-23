using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarElement : Abstract.BaseDbEntity
    {
        [Required]
        public int DynamicAvatarId { get; set; }

        [Required]
        public int DynamicAvatarLayerId { get; set; }
    }
}
