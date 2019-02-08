using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Language : Abstract.BaseDbEntity
    {
        [MaxLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
    }
}
