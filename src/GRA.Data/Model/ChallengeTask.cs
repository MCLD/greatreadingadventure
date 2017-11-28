using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeTask : Abstract.BaseDbEntity
    {
        [Required]
        public int ChallengeId { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(30)]
        public string Isbn { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }

        [Required]
        public int ChallengeTaskTypeId { get; set; }
    }
}
