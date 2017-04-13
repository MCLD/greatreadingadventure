using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Question : Abstract.BaseDbEntity
    {
        [Required]
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }
        public bool IsDeleted { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [Required]
        public string Text { get; set; }
        public int CorrectAnswerId { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
