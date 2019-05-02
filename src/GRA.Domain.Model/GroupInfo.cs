using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class GroupInfo : Abstract.BaseDomainEntity
    {
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.GroupName)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = ErrorMessages.Field)]
        [DisplayName(DisplayNames.GroupType)]
        public int GroupTypeId { get; set; }

        public GroupType GroupType { get; set; }

        [DisplayName(DisplayNames.GroupType)]
        public string GroupTypeName { get; set; }
    }
}
