using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class RecoveryToken : Abstract.BaseDbEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Token { get; set; }
    }
}
