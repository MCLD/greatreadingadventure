using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class SiteSetting : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }
        [MaxLength(255)]
        public string Value { get; set; }
    }
}
