using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Domain.Model.Filters
{
    public class DailyImageFilter : BaseFilter
    {
        public int DailyLiteracyTipId { get; set; }

        public DailyImageFilter(int? page = null) : base(page) { }
    }
}
