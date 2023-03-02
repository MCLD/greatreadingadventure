using System;
using System.Collections.Generic;

namespace GRA.Domain.Model
{
    public class ChangedItem<T>
    {
        public DateTime ChangedAt { get; set; }
        public int ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public ICollection<ObjectDifference> Differences { get; set; }
        public T NewItem { get; set; }
        public T OldItem { get; set; }
    }
}
