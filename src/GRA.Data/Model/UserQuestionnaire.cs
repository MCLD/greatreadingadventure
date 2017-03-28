using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class UserQuestionnaire
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
