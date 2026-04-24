using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarElement : Abstract.BaseDomainEntity
    {
        public string AltText { get; set; }

        public AvatarColor AvatarColor { get; set; }

        public int? AvatarColorId { get; set; }

        public AvatarItem AvatarItem { get; set; }

        [Required]
        public int AvatarItemId { get; set; }

        public string FilenameLink { get; set; }

        public int LayerId { get; set; }

        public int LayerPosition { get; set; }

        public string GetFilenameLink(int siteId, int layerId)
        {
            return AvatarColorId.HasValue
                ? $"site{siteId}/avatars/layer{layerId}/item{AvatarItemId}/item_{AvatarColorId}.png"
                : $"site{siteId}/avatars/layer{layerId}/item{AvatarItemId}/item.png";
        }
    }
}
