using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class GroupInfo : Abstract.BaseDbEntity
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int GroupTypeId { get; set; }
        public GroupType GroupType { get; set; }
    }
}
