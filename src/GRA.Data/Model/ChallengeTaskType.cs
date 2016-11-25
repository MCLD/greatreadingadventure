using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeTaskType : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
