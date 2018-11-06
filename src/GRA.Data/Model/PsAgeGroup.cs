using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsAgeGroup : Abstract.BaseDbEntity
    {
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
    }
}
