using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class PageHeader : Abstract.BaseDomainEntity
    {
        public ICollection<string> PageLanguages { get; set; }

        [Required]
        [DisplayName("Page Name")]
        public string PageName { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        [DisplayName("Slug")]
        public string Stub { get; set; }
    }
}
