using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Answer : Abstract.BaseDbEntity
    {
        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int SortOrder { get; set; }
        [MaxLength(1500)]
        [Required]
        public string Text { get; set; }
    }
}
