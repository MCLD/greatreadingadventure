using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class JoinCode : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }

        [Required]
        [MaxLength(8)]
        public string Code { get; set; }

        public bool IsQRCode { get; set; }

        public int? BranchId { get; set; }
        public Branch Branch { get; set; }

        public int AccessCount { get; set; }
        public int JoinCount { get; set; }
        
    }
}
