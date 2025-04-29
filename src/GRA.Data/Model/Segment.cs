using System;
using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class Segment : BaseDbEntity
    {
        public DateTime? EndDate { get; set; }
        public string Name { get; set; }
        public int SegmentType { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
