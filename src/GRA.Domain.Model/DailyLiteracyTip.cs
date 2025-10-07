﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DailyLiteracyTip : Abstract.BaseDomainEntity
    {
        public ICollection<DailyLiteracyTipImage> DailyLiteracyTipImages { get; set; }

        public bool IsLarge { get; set; }

        [MaxLength(50)]
        [Required]
        public string Message { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }

        [Required]
        public int SiteId { get; set; }
    }
}
