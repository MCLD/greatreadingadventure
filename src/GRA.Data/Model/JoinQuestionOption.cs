using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class JoinQuestionOption : Abstract.BaseDbEntity
    {
        public Domain.Model.JoinQuestion JoinQuestion { get; set; }
        [Required]
        public int JoinQuestionId { get; set; }
        public Segment Segment { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
