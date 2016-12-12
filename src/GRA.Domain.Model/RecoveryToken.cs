using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class RecoveryToken : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Token { get; set; }
    }
}
