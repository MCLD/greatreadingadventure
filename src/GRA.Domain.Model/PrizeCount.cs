namespace GRA.Domain.Model
{
    public class PrizeCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int? DrawingId { get; set; }
        public int? TriggerId { get; set; }
    }
}
