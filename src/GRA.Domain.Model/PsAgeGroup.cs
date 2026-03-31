using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsAgeGroup : Abstract.BaseDomainEntity
    {
        public ICollection<int> BackToBackBranchIds { get; set; }

        public double? ContrastRatio { get; set; }

        [Display(Name = "Icon Color")]
        [MaxLength(32)]
        [Required]
        public string IconColor { get; set; }

        public bool MeetsWCAG21AAContrastRequirement { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
    }
}
