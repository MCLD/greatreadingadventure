namespace GRA.Data.Model
{
    public class PsBackToBack
    {
        public int PsAgeGroupId { get; set; }
        public PsAgeGroup PsAgeGroup { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
