using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Category : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
