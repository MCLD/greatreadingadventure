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
        [DisplayName("Allow Back to Back")]
        public bool AllowBackToBack { get; set; }
    }
}
