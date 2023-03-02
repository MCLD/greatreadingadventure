namespace GRA.Domain.Model
{
    public class ObjectDifference
    {
        public string Property { get; set; }
        public object Value1 { get; set; }
        public string Value1Notes { get; set; }
        public object Value2 { get; set; }
        public string Value2Notes { get; set; }
    }
}
