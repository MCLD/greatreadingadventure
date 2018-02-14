using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class School : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [DisplayName("School District")]
        [Required]
        public int SchoolDistrictId { get; set; }
        public SchoolDistrict SchoolDistrict { get; set; }
        [DisplayName("School Type")]
        public int? SchoolTypeId { get; set; }
        public SchoolType SchoolType { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
