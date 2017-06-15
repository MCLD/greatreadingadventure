using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarBundle : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public bool CanBeUnlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasBeenAwarded { get; set; }

        public ICollection<DynamicAvatarItem> DynamicAvatarItems { get; set; }
    }
}
