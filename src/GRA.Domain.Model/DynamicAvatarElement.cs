using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarElement : Abstract.BaseDomainEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int Position { get; set; }
    }
}
