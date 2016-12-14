using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class Badge : BaseDbEntity
    {
        public int SiteId { get; set; }
        public string Filename { get; set; }
    }
}
