using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class GroupInfo : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Group name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Group type")]
        public int GroupTypeId { get; set; }
        public GroupType GroupType { get; set; }
        [DisplayName("Group type")]
        public string GroupTypeName { get; set; }
    }
}
