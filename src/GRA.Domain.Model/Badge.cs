using GRA.Domain.Model.Abstract;

namespace GRA.Domain.Model
{
    public class Badge : BaseDomainEntity
    {
        public int SiteId { get; set; }
        public string Filename { get; set; }
    }
}
