using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarItem : Abstract.BaseDomainEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }
        public int DynamicAvatarLayerPosition { get; set; }
        public string DynamicAvatarLayerName { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(255)]
        public string Thumbnail { get; set; }

        public bool Unlockable { get; set; }
    }
}
