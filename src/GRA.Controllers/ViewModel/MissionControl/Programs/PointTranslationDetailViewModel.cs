using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.Programs
{
    public class PointTranslationDetailViewModel
    {
        public PointTranslation PointTranslation { get; set; }
        public string Action { get; set; }
        public bool HasBeenUsed { get; set; }
    }
}
