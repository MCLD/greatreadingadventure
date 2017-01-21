namespace GRA.Domain.Model
{
    public class School : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int SchoolDistrictId { get; set; }
        public int SchoolTypeId { get; set; }
        public string Name { get; set; }
    }
}
