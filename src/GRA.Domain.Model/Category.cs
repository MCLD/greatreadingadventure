using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Category : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}