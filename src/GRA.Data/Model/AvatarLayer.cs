using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GRA.Data.Model
{
    public class AvatarLayer : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string RemoveLabel { get; set; }

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
        [Column(TypeName = "decimal(4,2)")]
        public decimal ZoomScale { get; set; }
        public int ZoomYOffset { get; set; }

        public ICollection<AvatarColor> AvatarColors { get; set; }
        public ICollection<AvatarItem> AvatarItems { get; set; }
    }
}
