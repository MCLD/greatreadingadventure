using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class EmailReminder : Abstract.BaseDbEntity
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string SignUpSource { get; set; }
    }
}
