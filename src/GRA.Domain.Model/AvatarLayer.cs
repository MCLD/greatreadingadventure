using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarLayer : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string SpanishName { get; set; }

        [MaxLength(255)]
        public string RemoveLabel { get; set; }

        [MaxLength(255)]
        public string SpanishRemoveLabel { get; set; }

        [Required]
        public int Position { get; set; }

        public bool CanBeEmpty { get; set; }
        public int GroupId { get; set; }
        public int SortOrder { get; set; }
        public bool DefaultLayer { get; set; }
        public bool ShowItemSelector { get; set; }
        public bool ShowColorSelector { get; set; }

        [MaxLength(255)]
        public string Icon { get; set; }

        public decimal ZoomScale { get; set; }
        public int ZoomYOffset { get; set; }

        public int? SelectedItem { get; set; }
        public int? SelectedColor { get; set; }
        public string FilePath { get; set; }

        public int AvailableItems { get; set; }
        public int UnavailableItems { get; set; }
        public int UnlockableItems { get; set; }

        public ICollection<AvatarColor> AvatarColors { get; set; }
        public ICollection<AvatarItem> AvatarItems { get; set; }
    }
}
