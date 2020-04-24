using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarBundle : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool CanBeUnlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasBeenAwarded { get; set; }
        public bool? HasBeenViewed { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int? AssociatedBundleId { get; set; }
        public ICollection<AvatarItem> AvatarItems { get; set; }
    }
}
