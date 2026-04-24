using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarLayer : Abstract.BaseDomainEntity
    {
        public string AltText { get; set; }
        public int AvailableItems { get; set; }
        public ICollection<AvatarColor> AvatarColors { get; set; }
        public ICollection<AvatarItem> AvatarItems { get; set; }
        public bool CanBeEmpty { get; set; }
        public bool DefaultLayer { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; }

        public int GroupId { get; set; }
        public string IconLink { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int Position { get; set; }

        [MaxLength(255)]
        public string RemoveLabel { get; set; }

        public int? SelectedColor { get; set; }

        public int? SelectedItem { get; set; }

        public bool ShowColorSelector { get; set; }

        public bool ShowItemSelector { get; set; }

        public int SiteId { get; set; }

        public int SortOrder { get; set; }

        public ICollection<AvatarLayerText> Texts { get; set; }

        public int UnavailableItems { get; set; }

        public int UnlockableItems { get; set; }

        public decimal ZoomScale { get; set; }

        public int ZoomYOffset { get; set; }

        public string GetIconLink()
        {
            return $"site{SiteId}/avatars/layer{Id}/icon.png";
        }
    }
}
