using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class StaticAvatar : BaseDbEntity
    {
        public int SiteId { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
