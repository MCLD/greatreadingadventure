using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipDetailViewModel : PaginateViewModel
    {
        public PaginateViewModel PaginateModel { get; set; }
        public ICollection<DailyLiteracyTipImageViewModel> Images { get; set; }
        public DailyLiteracyTip Tip { get; set; }
    }
}
