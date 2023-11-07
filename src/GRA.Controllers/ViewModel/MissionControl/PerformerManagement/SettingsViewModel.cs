using GRA.Domain.Model;

namespace GRA.Controllers.ViewModel.MissionControl.PerformerManagement
{
    public class SettingsViewModel : PerformerManagementPartialViewModel
    {
        public PsSettings Settings { get; set; }
        public int SiteId { get; set; }
    }
}
