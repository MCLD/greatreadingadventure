using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class TriggerChallenge
    {
        [Required]
        public int TriggerId { get; set; }
        public Trigger Trigger { get; set; }
        [Required]
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
