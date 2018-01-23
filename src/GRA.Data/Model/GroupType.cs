using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class GroupType : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
