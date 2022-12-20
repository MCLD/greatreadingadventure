using System.ComponentModel.DataAnnotations;
using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class Attachment : BaseDbEntity
    {
        public string FileName { get; set; }
        [Required]
        public bool? IsCertificate { get; set; } = true;

        public int SiteId { get; set; }
    }
}
