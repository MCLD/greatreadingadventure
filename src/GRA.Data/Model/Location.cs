using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Location : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }
        public string Address { get; set; }
        [MaxLength(50)]
        public string Telephone { get; set; }
    }
}
