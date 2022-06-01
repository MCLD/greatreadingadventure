using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailBase : Abstract.BaseDomainEntity
    {
        public IEnumerable<int> ConfiguredLanguages { get; set; }
        public EmailBaseText EmailBaseText { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
