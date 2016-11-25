using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class UserRole : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
}
