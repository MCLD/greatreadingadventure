using System;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class KitViewModel
    {
        public PsKit Kit { get; set; }
        public string ImagePath { get; set; }
        public Uri Uri { get; set; }

        public int? NextKit { get; set; }
        public int? PrevKit { get; set; }
        public int ReturnPage { get; set; }

        public bool SchedulingOpen { get; set; }
        public SelectList AgeGroupList { get; set; }
        public SelectList BranchList { get; set; }
        public PsBranchSelection BranchSelection { get; set; }

        public bool CanSchedule { get; set; }
    }
}
