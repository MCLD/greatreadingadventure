using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class JoinQuestion : Abstract.BaseDbEntity
    {
        public Segment Segment { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
