using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Drawing : Abstract.BaseDomainEntity
    {
        [Required]
        public int DrawingCriteriaId { get; set; }
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
