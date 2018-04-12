using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AvatarElement : Abstract.BaseDomainEntity
    {
        [Required]
        public int AvatarItemId { get; set; }
        public AvatarItem AvatarItem { get; set; }

        public int? AvatarColorId { get; set; }
        public AvatarColor AvatarColor { get; set; }

        [MaxLength(255)]
        [Required]
        public string Filename { get; set; }
    }
}
