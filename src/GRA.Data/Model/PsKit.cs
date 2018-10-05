using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class PsKit : Abstract.BaseDbEntity
    {
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Website { get; set; }

        public IList<PsKitImage> Images { get; set; }
        public ICollection<PsKitAgeGroup> AgeGroups { get; set; }
    }
}
