using System.ComponentModel.DataAnnotations;
using GRA.Domain.Model.Abstract;

namespace GRA.Domain.Model
{
    public class Attachment : BaseDomainEntity
    {
        public string FileName { get; set; }
        [Required]
        public bool? IsCertificate { get; set; }

        public int SiteId { get; set; }
    }
}
