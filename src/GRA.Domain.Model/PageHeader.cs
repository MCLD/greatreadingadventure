using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PageHeader : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public string PageName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Stub { get; set; }
    }
}
