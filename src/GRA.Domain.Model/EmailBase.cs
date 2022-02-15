using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailBase : Abstract.BaseDomainEntity
    {
        public EmailBaseText EmailBaseText { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
