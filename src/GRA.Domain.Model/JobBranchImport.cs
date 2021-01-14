namespace GRA.Domain.Model
{
    public class JobBranchImport
    {
        public bool DoImport { get; set; }
        public string Filename { get; set; }
        public int UserId { get; set; }
    }
}
