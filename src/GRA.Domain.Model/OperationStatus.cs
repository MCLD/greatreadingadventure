namespace GRA.Domain.Model
{
    public class OperationStatus
    {
        public string Title { get; set; }
        public int? PercentComplete { get; set; }
        public string Status { get; set; }
        public bool Complete { get; set; }
        public bool Error { get; set; }
    }
}
