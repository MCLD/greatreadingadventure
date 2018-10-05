namespace GRA.Data.Model
{
    public class PsKitAgeGroup
    {
        public int KitId { get; set; }
        public PsKit Kit { get; set; }
        public int AgeGroupId { get; set; }
        public PsAgeGroup AgeGroup { get; set; }
    }
}
