using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatar : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int Position { get; set; }
        public virtual ICollection<DynamicAvatarElement> Elements { get; set; }
    }
}
