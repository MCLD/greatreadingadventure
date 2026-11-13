using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JoinQuestion : Abstract.BaseDomainEntity
    {
        public Segment Segment { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
