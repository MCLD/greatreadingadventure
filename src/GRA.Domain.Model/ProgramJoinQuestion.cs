using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class ProgramJoinQuestion
    {
        public Program Program { get; set; }
        [Required]
        public int ProgramId { get; set; }
        public JoinQuestion JoinQuestion { get; set; }
        [Required]
        public int JoinQuestionId { get; set; }
        [Required]
        public bool IsRequired { get; set; }
    }
}
