using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JoinCode : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(8)]
        public string Code { get; set; }

        public bool IsQRCode { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public int? BranchSystemId { get; set; }
        public string BranchSystemName { get; set; }
        public int AccessCount { get; set; }
        public int JoinCount { get; set; }

        public string JoinUrl { get; set; }
        public bool NewCode { get; set; }
    }
}
