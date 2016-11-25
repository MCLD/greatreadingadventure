using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Permission : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
