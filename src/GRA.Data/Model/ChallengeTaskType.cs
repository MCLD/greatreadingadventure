using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeTaskType : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int? ActivityCount { get; set; }
        public int? PointTranslationId { get; set; }
    }
}
