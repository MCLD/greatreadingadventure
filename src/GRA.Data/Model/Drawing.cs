using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Drawing : Abstract.BaseDbEntity
    {
        [Required]
        public int DrawingCriteriaId { get; set; }
        public DrawingCriterion DrawingCriteria { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        [Required]
        public string RedemptionInstructions { get; set; }
        [Required]
        public int WinnerCount { get; set; }
    }
}
