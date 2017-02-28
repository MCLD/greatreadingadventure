
namespace GRA.Domain.Model
{
    public class DataWithId<DataType>
    {
        public DataType Data { get; set; }
        public int Id { get; set; }
    }
}
