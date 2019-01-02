using System;
using System.Collections.Generic;
using System.Text;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class KitSelectionsViewModel
    {
        public PsKit Kit { get; set; }
        public ICollection<PsBranchSelection> BranchSelections { get; set; }

        public int? NextKit { get; set; }
        public int? PrevKit { get; set; }
        public int ReturnPage { get; set; }

        public SelectList KitList { get; set; }
        public SelectList AgeGroupList { get; set; }
        public PsBranchSelection BranchSelection { get; set; }
    }
}
