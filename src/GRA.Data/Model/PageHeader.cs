using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PageHeader : Abstract.BaseDbEntity
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
