using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Model
{
    public class ChallengeTask : Abstract.BaseDbEntity
    {
        [Required]
        public int ChallengeId { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(30)]
        public string Isbn { get; set; }

        [MaxLength(500)]
        public string Url { get; set; }

        [Required]
        public int ChallengeTaskTypeId { get; set; }
    }
}
