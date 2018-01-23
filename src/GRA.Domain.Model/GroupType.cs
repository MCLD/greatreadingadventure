using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class GroupType : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Group type")]
        public string Name { get; set; }
    }
}
