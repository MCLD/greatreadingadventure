using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class School : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public int SchoolDistrictId { get; set; }
        public virtual SchoolDistrict SchoolDistrict { get; set; }

        [Required]
        public int SchoolTypeId { get; set; }
        public virtual SchoolType SchoolType { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
