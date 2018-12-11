using System;
using System.Collections.Generic;
using System.Text;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class KitListViewModel : PerformerManagementPartialViewModel
    {
        public List<PsKit> Kits { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }
        public PsKit KitToDelete { get; set; }
    }
}
