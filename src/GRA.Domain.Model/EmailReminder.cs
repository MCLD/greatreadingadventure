using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class EmailReminder : Abstract.BaseDomainEntity
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string SignUpSource { get; set; }
    }
}
