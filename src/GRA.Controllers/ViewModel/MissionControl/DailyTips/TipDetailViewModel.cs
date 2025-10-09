using System;
using System.Collections.Generic;
using System.Globalization;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipDetailViewModel : PaginateViewModel
    {
        public TipDetailViewModel()
        {
            CacheBuster = DateTime.Now.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);
            Images = [];
            Paths = new Dictionary<int, string>();
        }

        public string CacheBuster { get; set; }
        public Tuple<int, int> FirstAndLast { get; set; }
        public IList<DailyLiteracyTipImage> Images { get; }
        public IDictionary<int, string> Paths { get; }
        public DailyLiteracyTip Tip { get; set; }
    }
}
