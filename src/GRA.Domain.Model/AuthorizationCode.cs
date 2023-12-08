using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class AuthorizationCode : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Code { get; set; }
        public string Description { get; set; }
        [DisplayName("Role Program")]
        public int? ProgramId { get; set; }
        [DisplayName("Assigned Branch")]
        public int? BranchId { get; set; }
        [Required]
        [DisplayName("Role")]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int Uses { get; set; }
        [DisplayName("Single Use")]
        public bool IsSingleUse { get; set; }
        [DisplayName("Express Sign-Up")]
        public bool SinglePageSignUp { get; set; }
    }
}
