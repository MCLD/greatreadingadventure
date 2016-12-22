using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class ChallengeCategory
    {
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}