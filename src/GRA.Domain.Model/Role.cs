using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Role : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int PermissionCount { get; set; }
    }
}
