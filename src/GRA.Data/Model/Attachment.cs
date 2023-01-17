using System.ComponentModel.DataAnnotations;
using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class Attachment : BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        public bool? IsCertificate { get; set; } = true;

        [Required]
        public int SiteId { get; set; }
    }
}
