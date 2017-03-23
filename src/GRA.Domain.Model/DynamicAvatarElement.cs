using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarElement : Abstract.BaseDomainEntity
    {
        [Required]
        public int DynamicAvatarId { get; set; }

        [Required]
        public int DynamicAvatarLayerId { get; set; }
    }
}
