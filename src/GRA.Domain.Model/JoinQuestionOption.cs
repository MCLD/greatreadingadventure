using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JoinQuestionOption : Abstract.BaseDomainEntity
    {
        public JoinQuestion JoinQuestion { get; set; }
        [Required]
        public int JoinQuestionId { get; set; }
        public Segment Segment { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
