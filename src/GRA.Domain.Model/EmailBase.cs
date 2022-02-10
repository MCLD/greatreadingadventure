using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailBase : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
