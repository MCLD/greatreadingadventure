using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class SpatialDistanceHeader : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }

        [MaxLength(50)]
        public string Geolocation { get; set; }

        public bool IsValid { get; set; }
    }
}
