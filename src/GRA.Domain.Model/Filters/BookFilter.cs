namespace GRA.Domain.Model.Filters
{
    public class BookFilter : BaseFilter
    {
        public SortBooksBy SortBy { get; set; }
        public bool OrderDescending { get; set; }

        public BookFilter(int? page = null) : base(page) { }
    }

    public enum SortBooksBy
    {
        Date,
        Title,
        Author
    }
}
