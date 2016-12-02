using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class AuthorizationCode : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Code { get; set; }
        public string Description { get; set; }
        [Required]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int Uses { get; set; }
        public bool IsSingleUse { get; set; }
    }
}
