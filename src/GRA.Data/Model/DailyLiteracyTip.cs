using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DailyLiteracyTip : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string Message { get; set; }

        public bool IsLarge { get; set; }

        public ICollection<DailyLiteracyTipImage> DailyLiteracyTipImages { get; set; }
    }
}
