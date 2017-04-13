using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Question : Abstract.BaseDomainEntity
    {
        public int QuestionnaireId { get; set; }
        public bool IsDeleted { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [Required]
        public string Text { get; set; }
        public int CorrectAnswerId { get; set; }

        public ICollection<Answer> Answers;

        public int ParticipantAnswer { get; set; }
    }
}
