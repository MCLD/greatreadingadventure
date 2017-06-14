namespace GRA.Domain.Model.Filters
{
    public class TriggerFilter : BaseFilter
    {
        public bool? SecretCodesOnly { get; set; }

        public TriggerFilter(int? page = null, int take = 10) : base(page, take) { }
    }
}
