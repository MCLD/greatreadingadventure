using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class TipListViewModel
    {
        public ICollection<DailyLiteracyTip> DailyTips { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public IDictionary<int, int> TipCount { get; set; }
    }
}