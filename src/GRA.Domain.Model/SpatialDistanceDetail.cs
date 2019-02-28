namespace GRA.Domain.Model
{
    public class SpatialDistanceDetail
    {
        public int Id { get; set; }

        public int SpatialDistanceHeaderId { get; set; }

        public int? BranchId { get; set; }

        public int? LocationId { get; set; }

        public double Distance { get; set; }
    }
}
