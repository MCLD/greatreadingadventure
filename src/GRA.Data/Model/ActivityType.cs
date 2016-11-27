using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ActivityType
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
