using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Language : Abstract.BaseDomainEntity
    {
        [MaxLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
    }
}
