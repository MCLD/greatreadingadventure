using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class DynamicAvatar : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public int Position { get; set; }
        public IEnumerable<DynamicAvatarElement> Elements { get; set; }
    }
}
