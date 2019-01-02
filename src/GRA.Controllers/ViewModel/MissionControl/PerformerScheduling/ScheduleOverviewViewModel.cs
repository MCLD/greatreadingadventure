using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class ScheduleOverviewViewModel
    {
        public PsSettings Settings { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }

        public IEnumerable<Branch> Branches { get; set; }
        public ICollection<PsAgeGroup> AgeGroups { get; set; }

        public string SystemName { get; set; }

        public SelectList BranchList { get; set; }

        public bool CanSchedule { get; set; }
    }
}
