using System;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerSelectionsViewModel
    {
        public PsPerformer Performer { get; set; }
        public List<List<PsBranchSelection>> BranchSelectionDates { get; set; }

        public int? NextPerformer { get; set; }
        public int? PrevPerformer { get; set; }
        public int ReturnPage { get; set; }

        public DateTime DefaultPerformerScheduleStartTime { get; set; }
        public DateTime DefaultPerformerScheduleEndTime { get; set; }
    }
}
