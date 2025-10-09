using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DailyLiteracyTipImage : Abstract.BaseDomainEntity
    {
        public int DailyLiteracyTipId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(10)]
        [Required]
        public string Extension { get; set; }

        public int Day { get; set; }
    }
}
