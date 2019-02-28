namespace GRA.Data.Model
{
    public class SpatialDistanceDetail
    {
        public int Id { get; set; }

        public int SpatialDistanceHeaderId { get; set; }
        public SpatialDistanceHeader SpatialDistanceHeader { get; set; }

        public int? BranchId { get; set; }
        public Branch Branch { get; set; }

        public int? LocationId { get; set; }
        public Location Location { get; set; }

        public double Distance { get; set; }
    }
}
