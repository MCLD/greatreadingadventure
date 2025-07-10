using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.DailyTips
{
    public class DailyLiteracyTipImageViewModel
    {
        public int Id { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ImagePath { get; set; }
    }
}
