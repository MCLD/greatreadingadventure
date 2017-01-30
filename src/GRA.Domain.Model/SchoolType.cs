using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class SchoolType : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
