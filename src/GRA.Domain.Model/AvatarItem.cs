using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarItem : Abstract.BaseDomainEntity
    {
        public string AltText { get; set; }

        [Required]
        public int AvatarLayerId { get; set; }

        public string AvatarLayerName { get; set; }
        public int AvatarLayerPosition { get; set; }
        public string FileType { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }
        public ICollection<AvatarItemText> Texts { get; set; }
        public string ThumbnailLink { get; set; }
        public bool Unlockable { get; set; }

        public string GetThumbnailLink(int siteId)
        {
            return $"site{siteId}/avatars/layer{AvatarLayerId}/item{Id}/thumbnail.jpg";
        }
    }
}
