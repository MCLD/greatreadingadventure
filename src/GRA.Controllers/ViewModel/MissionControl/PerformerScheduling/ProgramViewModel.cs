using System.Collections.Generic;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class ProgramViewModel
    {
        public PsProgram Program { get; set; }
        public string Image { get; set; }

        public bool List { get; set; }
        public PsAgeGroup AgeGroup { get; set; }
        public int? NextProgram { get; set; }
        public int? PrevProgram { get; set; }
        public int ReturnPage { get; set; }

        public GRA.Domain.Model.System System { get; set; }
        public bool AllBranches { get; set; }
        public ICollection<int> BranchAvailability { get; set; }

        public bool SchedulingOpen { get; set; }
        public SelectList AgeGroupList { get; set; }
        public SelectList BranchList { get; set; }
        public PsBranchSelection BranchSelection { get; set; }
    }
}
