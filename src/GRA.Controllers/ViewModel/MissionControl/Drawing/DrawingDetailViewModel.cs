using GRA.Controllers.ViewModel.Shared;

namespace GRA.Controllers.ViewModel.MissionControl.Drawing
{
    public class DrawingDetailViewModel
    {
        public GRA.Domain.Model.Drawing Drawing { get; set; }
        public string CreatedByName { get; set; }
        public bool CanMailWinners { get; set; }
        public bool CanViewParticipants { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
