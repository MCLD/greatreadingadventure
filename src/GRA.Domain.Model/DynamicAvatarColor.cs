using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarColor : Abstract.BaseDomainEntity
    {
        [Required]
        public int DynamicAvatarLayerId { get; set; }

        [MaxLength(15)]
        [Required]
        public string Color { get; set; }
        public int SortOrder { get; set; }
    }
}
