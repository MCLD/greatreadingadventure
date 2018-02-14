using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class SchoolDistrict : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsCharter { get; set; }
        public bool IsPrivate { get; set; }

        public virtual ICollection<School> Schools { get; set; }
    }
}
