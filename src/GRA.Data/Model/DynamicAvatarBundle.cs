using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarBundle : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public bool CanBeUnlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasBeenAwarded { get; set; }

        public ICollection<DynamicAvatarBundleItem> DynamicAvatarBundleItems { get; set; }
    }
}
