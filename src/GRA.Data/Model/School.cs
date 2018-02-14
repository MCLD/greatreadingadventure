using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class School : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public int SchoolDistrictId { get; set; }
        public SchoolDistrict SchoolDistrict { get; set; }

        public int? SchoolTypeId { get; set; }
        public SchoolType SchoolType { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
