using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Drawing> Drawings { get; set; }
        public PaginateViewModel PaginateModel { get; set; }

        public bool? Archived { get; set; }
        public string Search { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? ProgramId { get; set; }
        public bool? Mine { get; set; }
        public string ActiveNav { get; set; }
        public string SystemName { get; set; }
        public string BranchName { get; set; }
        public string ProgramName { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
        public IEnumerable<GRA.Domain.Model.Program> ProgramList { get; set; }
    }
}
