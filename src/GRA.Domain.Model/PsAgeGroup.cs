using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsAgeGroup : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Icon Color",
            Description = "a Web color entered in hexadecimal such as #4682b4")]
        [MaxLength(32)]
        [Required]
        public string IconColor { get; set; }

        public ICollection<int> BackToBackBranchIds { get; set; }
    }
}
