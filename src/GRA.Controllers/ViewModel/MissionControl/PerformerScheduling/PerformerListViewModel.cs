using System.Collections.Generic;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerScheduling
{
    public class PerformerListViewModel
    {
        public ICollection<PsPerformer> Performers { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
