using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class ICollectionWithCount<TItem>
    {
        public int Count { get; set; }
        public ICollection<TItem> Data { get; set; }
    }
}
