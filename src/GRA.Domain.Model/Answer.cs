using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Answer : Abstract.BaseDomainEntity
    {
        public int QuestionId { get; set; }
        public int SortOrder { get; set; }
        [MaxLength(1500)]
        [Required]
        public string Text { get; set; }
    }
}
