using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JoinQuestionOption : Abstract.BaseDomainEntity
    {
        public int DisplayOrder { get; set; }
        [Required]
        public int JoinQuestionId { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
