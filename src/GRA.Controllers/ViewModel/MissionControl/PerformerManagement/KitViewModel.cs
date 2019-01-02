using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class KitViewModel
    {
        public PsKit Kit { get; set; }
        public string ImagePath { get; set; }
        public Uri Uri { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }

        public int? NextKit { get; set; }
        public int? PrevKit { get; set; }
        public int ReturnPage { get; set; }
    }
}
