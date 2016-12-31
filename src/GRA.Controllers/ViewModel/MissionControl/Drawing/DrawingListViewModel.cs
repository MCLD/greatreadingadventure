using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingListViewModel
    {
        public IEnumerable<GRA.Domain.Model.Drawing> Drawings { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
