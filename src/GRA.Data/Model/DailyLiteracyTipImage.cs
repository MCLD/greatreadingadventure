using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DailyLiteracyTipImage : Abstract.BaseDbEntity
    {
        public int DailyLiteracyTipId { get; set; }
        public DailyLiteracyTip DailyLiteracyTip { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(10)]
        [Required]
        public string Extension { get; set; }

        public int Day { get; set; }
    }
}
