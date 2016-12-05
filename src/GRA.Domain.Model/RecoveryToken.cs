using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class RecoveryToken : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId;
        [Required]
        [MaxLength(255)]
        public string Token { get; set; }

    }
}
