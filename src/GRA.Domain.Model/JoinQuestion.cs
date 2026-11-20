using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class JoinQuestion : Abstract.BaseDomainEntity
    {
        [Required]
        public int DisplayOrder { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsRequired { get; set; }
        [Required]
        public int SegmentId { get; set; }
    }
}
