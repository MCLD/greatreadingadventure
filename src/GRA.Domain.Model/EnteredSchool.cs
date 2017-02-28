
namespace GRA.Domain.Model
{
    public class EnteredSchool : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int SchoolDistrictId { get; set; }
        public string Name { get; set; }
    }
}
