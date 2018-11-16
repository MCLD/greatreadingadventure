using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class PerformerListViewModel : PerformerManagementPartialViewModel
    {
        public List<PsPerformer> Performers { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
        public PsPerformer PerformerToDelete { get; set; }
        public bool RegistrationClosed { get; set; }
        public PsSchedulingStage SchedulingStage { get; set; }
    }
}
