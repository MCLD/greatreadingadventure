using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class CriterionListViewModel
    {
        public IEnumerable<GRA.Domain.Model.DrawingCriterion> Criteria { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
