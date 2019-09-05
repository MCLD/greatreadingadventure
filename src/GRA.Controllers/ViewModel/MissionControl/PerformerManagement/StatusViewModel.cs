using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class StatusViewModel : PerformerManagementPartialViewModel
    {
        public DateTime Now { get; set; }
        public PsSettings Settings { get; set; }
        public ICollection<Domain.Model.System> Systems { get; set; }
        public Dictionary<int, string> Completion { get; set; }
        public Dictionary<int, string> Percent { get; set; }
        public Dictionary<int, string> Panel { get; set; }
        public string SummaryPercent { get; set; }
        public int PerformerCount { get; set; }
        public int ProgramCount { get; set; }
        public int KitCount { get; set; }
        public string SchedulingStage { get; set; }

        public string CollapsePanel
        {
            get
            {
                return Systems.Count != 1
                    ? "collapse"
                    : "";
            }
        }
    }
}
