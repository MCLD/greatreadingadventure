using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PsAgeGroup : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [DisplayName("Icon Color")]
        [MaxLength(32)]
        [Required]
        public string IconColor { get; set; }
        
        public ICollection<int> BackToBackBranchIds { get; set; }
    }
}
