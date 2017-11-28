using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Category : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(10)]
        public string Color { get; set; }
    }
}
