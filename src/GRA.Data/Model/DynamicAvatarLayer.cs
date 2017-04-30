using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarLayer : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int Position { get; set; }
        public bool CanBeEmpty { get; set; }
        public int GroupId { get; set; }
        public int SortOrder { get; set; }
        public bool DefaultLayer { get; set; }
        public bool ShowItemSelector { get; set; }
        public bool ShowColorSelector { get; set; }

        public ICollection<DynamicAvatarColor> DynamicAvatarColors { get; set; }
        public ICollection<DynamicAvatarItem> DynamicAvatarItems { get; set; }
    }
}
