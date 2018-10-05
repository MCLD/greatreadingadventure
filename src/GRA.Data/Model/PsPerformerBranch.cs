namespace GRA.Data.Model
{
    public class PsPerformerBranch
    {
        public int PerformerId { get; set; }
        public PsPerformer Performer { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
