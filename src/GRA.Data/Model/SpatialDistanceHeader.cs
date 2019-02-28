using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class SpatialDistanceHeader : Abstract.BaseDbEntity
    {
        public int SiteId { get; set; }

        [MaxLength(50)]
        public string Geolocation { get; set; }

        public bool IsValid { get; set; }
    }
}
