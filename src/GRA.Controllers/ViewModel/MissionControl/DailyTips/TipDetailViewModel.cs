using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipDetailViewModel
    {
        public PaginateViewModel PaginateModel { get; set; }
        public ICollection<DailyLiteracyTipImage> Images { get; set; }
        public DailyLiteracyTip Tip { get; set; }
    }
}
