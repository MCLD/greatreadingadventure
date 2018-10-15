namespace GRA.Data.Model
{
    public class PsPerformerBranch
    {
        public int PsPerformerId { get; set; }
        public PsPerformer PsPerformer { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
