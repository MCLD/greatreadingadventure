using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarItem : Abstract.BaseDomainEntity
    {
        [Required]
        public int AvatarLayerId { get; set; }
        public int AvatarLayerPosition { get; set; }
        public string AvatarLayerName { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(255)]
        public string Thumbnail { get; set; }

        public bool Unlockable { get; set; }
    }
}
