using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarColor : Abstract.BaseDomainEntity
    {
        [Required]
        public int AvatarLayerId { get; set; }

        [MaxLength(15)]
        [Required]
        public string Color { get; set; }
        public int SortOrder { get; set; }
        public ICollection<AvatarColorText> Texts { get; set; }

        public string AltText { get; set; }
    }
}
