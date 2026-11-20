using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class JoinQuestionOption : Abstract.BaseDbEntity
    {
        public int DisplayOrder { get; set; }
        [Required]
        public int JoinQuestionId { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
