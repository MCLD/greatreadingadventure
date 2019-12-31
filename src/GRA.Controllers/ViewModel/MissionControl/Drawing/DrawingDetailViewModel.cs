using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingDetailViewModel
    {
        public GRA.Domain.Model.Drawing Drawing { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
